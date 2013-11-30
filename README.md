# Simple YUI Compressor .NET

## Introduction

Simple YUI Compressor .NET is a .NET 2.0-compatible library for combining and minifying JavaScript and CSS files for display on websites. Usage of SimpleYUI is similar to [SquishIt](https://github.com/jetheredge/SquishIt). SimpleYUI is not intended to be a replacement to SquishIt, but rather is intended to provide a simpler, less feature-rich solution that's compatible with .NET 2.0 and up. "Simple" in this case means:

- Just one .dll file to include
- Only supports pure JS and CSS (no support for LESS, SASS, CofeeScript, etc. If you need those, use SquishIt)
- Only supports one compression engine (YUI)
- No configuration required

Simple YUI Compressor .NET is in no way related to [SimpleYUI](http://www.yuiblog.com/blog/2010/09/03/coming-inyui-3-2-0-simpleyui/)

## How To Use

First, you have to reference SimpleYUI.dll.

An example of using SimpleYUI is:

```aspx
<!DOCTYPE html>
<html>
    <head>
	    <title>SimpleYUI Demo</title>
		<%= SimpleYUI.Bundler.CSS()
		        .Add("~/css/style1.css")
				.Add("~/css/style2.css")
				.Render("~/css/combined.css") %>
		<%= SimpleYUI.Bundler.JavaScript()
		        .Add("~/scripts/script1.js")
				.Add("~/scripts/script2.js")
				.Render("~/scripts/combined.js") %>
	</head>
	<body>
	</body>
</html>
```

SimpleYUI injects a hash into the rendered filename based on the parameters passed and the last modified times of the included files. Therefore SimpleYUI will not regenerate the combined file on every request (which would be extremely inefficient) but only when one of the included files is changed, or the parameters passed to CSS() or JavaScript() change. When debugging, you can have SimpleYUI output individual <script>/<link> tags for each file instead of the combined file by including debug=true in the request query string (e.g. http://www.example.com/index.aspx?debug=true) or setting the "debug" parameter, which is the first parameter of the `CSS()` and `JavaScript()` methods to true (e.g. `SimpleYUI.Bundler.CSS(true)`)

Note that relative paths in CSS files will not be converted to work in the combined CSS file, so you may need to use absolute paths unless the combined CSS file is going to be outputted to the same directory as the CSS file(s) containing relative paths.

You can pass a number of parameters to `CSS()` and `JavaScript()` which will change how SimpleYUI is configured. By default SimpleYUI uses the most aggressive compression settings.

### CSS() Parameters

##### debug (bool)

If set to true, no combining/compression will take place and <link> tags to each individual CSS file will be output. If no parameter is passed to debug, it's possible to trigger debug mode by adding debug=true to the request query string.

##### useCompression (bool)

If set to true, SimpleYUI will compress the CSS by removing superfluous whitespace. If set to false, no compression will take place (but files will still be combined into one.) Defaults to true.

##### removeComments (bool)

If set to true, SimpleYUI will remove comments from the CSS. If set to false, it won't. Defaults to true.

##### lineBreakPosition (int)

Roughly how many characters should be included per line in the compressed CSS file. Defaults to -1 (no line breaks.) Note that a line break will always be added after the contents of each file regardless of this setting.

### JavaScript() Parameters

##### debug (bool)

If set to true, no combining/compression will take place and <script> tags to each individual JS file will be output. If no parameter is passed to debug, it's possible to trigger debug mode by adding debug=true to the request query string.

##### useCompression (bool)

If set to true, SimpleYUI will compress the JavaScript by removing superfluous whitespace. If set to false, no compression will take place (but files will still be combined into one.) Defaults to true.

##### obfuscate (bool)

If set to true, SimpleYUI will obfuscate the JavaScript (by shortening variable names, etc.) If set to false, no obfuscation will take place. This may need to be set to false for code that uses certain JavaScript libraries (e.g. AngularJS.) Defaults to true.

##### preserveSemicolons (bool)

If set to true, SimpleYUI will not remove any semicolons. Defaults to false.

##### disableOptimizations (bool)

If set to true, SimpleYUI will not make any micro-optimizations (e.g. conversion of `foo['bar']` to `foo.bar`.) Defaults to false.

##### ignoreEval (bool)

If set to true, SimpleYUI won't touch code in eval() statements. Defaults to false.

##### lineBreakPosition (int)

Roughly how many characters should be included per line in the compressed JS file. Defaults to -1 (no line breaks.) Note that a line break will always be added after the contents of each file regardless of this setting.

## Planned Features

- Total JavaScript comment removal.
- Conversion of relative paths in combined CSS files.
- Test cases.