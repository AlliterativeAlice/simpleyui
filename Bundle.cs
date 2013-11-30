using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Web;
using System.Web.UI;
using Yahoo.Yui.Compressor;

namespace SimpleYUI
{
    public class Bundle
    {
        internal enum BundleType
        {
            CSS,
            JavaScript
        }

        
        private List<string> files = new List<string>();
        private List<string> webFiles = new List<string>();
        private BundleType Type;
        private bool Debug = false;
        private bool UseCompression = true;
        private bool Obfuscate = true;
        private bool PreserveSemicolons = false;
        private bool DisableOptimizations = false;
        private bool IgnoreEval = false;
        private bool RemoveComments = true;
        private int LineBreakPosition = -1;

        internal Bundle(BundleType _type, bool _debug)
        {
            Type = _type;
            Debug = _debug;
        }

        internal Bundle(BundleType _type, bool _debug, bool _useCompression, bool _obfuscate, bool _preserveSemicolons, bool _disableOptimizations, bool _ignoreEval, int _lineBreakPosition)
        {
            //Only used for JS
            Type = _type;
            Debug = _debug;
            UseCompression = _useCompression;
            Obfuscate = _obfuscate;
            PreserveSemicolons = _preserveSemicolons;
            DisableOptimizations = _disableOptimizations;
            IgnoreEval = _ignoreEval;
            LineBreakPosition = _lineBreakPosition;
        }

        internal Bundle(BundleType _type, bool _debug, bool _useCompression, bool _removeComments, int _lineBreakPosition)
        {
            //Only used for CSS
            Type = _type;
            Debug = _debug;
            UseCompression = _useCompression;
            RemoveComments = _removeComments;
            LineBreakPosition = _lineBreakPosition;
        }

        public Bundle Add(string file)
        {
            string path = HttpContext.Current.Server.MapPath(file);
            if (files.Contains(path)) return this;
            if (File.Exists(path)) files.Add(path);
            else throw new FileNotFoundException("File \"" + file + "\" passed to SimpleYUI Bundler not found", path);
            webFiles.Add(file);
            return this;
        }

        public string Render(string outputFile)
        {
            if (Debug)
            {
                string output = String.Empty;
                foreach (string file in webFiles)
                {
                    string outputPath = (HttpContext.Current.Handler as Page).ResolveUrl(file);
                    if (Type == BundleType.CSS)
                        output += MakeCSSTag(outputPath) + "\r\n";
                    else
                        output += MakeJSTag(outputPath) + "\r\n";
                }
                return output;
            }
            else
            {
                string fileAges = String.Empty;
                foreach (string file in files) fileAges += file + File.GetLastWriteTime(file).ToString();
                fileAges = AppendOptionsAsString(fileAges); //So that file will be regenerated if options are changed
                string hash = CalculateMD5Hash(fileAges);
                string fileWithHash = InjectHash(outputFile, hash);
                string fileOnSystem = HttpContext.Current.Server.MapPath(fileWithHash);
                string outputWebPath = (HttpContext.Current.Handler as Page).ResolveUrl(fileWithHash);
                if (!File.Exists(fileOnSystem))
                {
                    //Do CSS/JS Compression
                    string totalContents = String.Empty;
                    CssCompressor cssCompressor = null;
                    JavaScriptCompressor jsCompressor = null;
                    if (Type == BundleType.CSS)
                    {
                        cssCompressor = new CssCompressor();
                        cssCompressor.CompressionType = (UseCompression ? CompressionType.Standard : CompressionType.None);
                        cssCompressor.RemoveComments = RemoveComments;
                        cssCompressor.LineBreakPosition = LineBreakPosition;
                    }
                    else
                    {
                        jsCompressor = new JavaScriptCompressor();
                        jsCompressor.CompressionType = (UseCompression ? CompressionType.Standard : CompressionType.None);
                        jsCompressor.DisableOptimizations = DisableOptimizations;
                        jsCompressor.IgnoreEval = IgnoreEval;
                        jsCompressor.LineBreakPosition = LineBreakPosition;
                        jsCompressor.ObfuscateJavascript = Obfuscate;
                        jsCompressor.PreserveAllSemicolons = PreserveSemicolons;
                    }
                    foreach (string file in files) totalContents += File.ReadAllText(file) + "\n";
                    totalContents = (Type == BundleType.CSS ? cssCompressor.Compress(totalContents) : jsCompressor.Compress(totalContents));
                    File.WriteAllText(fileOnSystem, totalContents);
                }
                return MakeTag(outputWebPath) + "\r\n";
            }
        }

        private string InjectHash(string file, string hash)
        {
            string[] splitPath = file.Split('/');
            string fileName = splitPath[splitPath.Length - 1];
            int lastDotInd = fileName.LastIndexOf('.');
            if (lastDotInd == -1) return file + "_" + hash + (Type == BundleType.CSS ? ".css" : ".js");
            else
            {
                splitPath[splitPath.Length - 1] = fileName.Substring(0, lastDotInd) + "_" + hash + (Type == BundleType.CSS ? ".css" : ".js");
                return String.Join("/", splitPath);
            }
        }

        private string MakeCSSTag(string file)
        {
            return "<link rel=\"stylesheet\" href=\"" + file + "\" />";
        }

        private string MakeJSTag(string file)
        {
            return "<script type=\"text/javascript\" src=\"" + file + "\"></script>";
        }

        private string MakeTag(string file)
        {
            return (Type == BundleType.CSS ? MakeCSSTag(file) : MakeJSTag(file));
        }

        private string CalculateMD5Hash(string input)
        {
            // step 1, calculate MD5 hash from input
            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }

        private string AppendOptionsAsString(string str)
        {
            return str + UseCompression + Obfuscate + PreserveSemicolons + DisableOptimizations + IgnoreEval + RemoveComments + LineBreakPosition;
        }
    }
}
