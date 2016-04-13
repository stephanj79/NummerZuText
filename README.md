# NummerZuText
```cs
 /// Als String
 var dou = Convert.ToDouble(textBox1.Text);
 var s = textBox1.Text.NummerZuText(true,true);
 label1.Text = s;
 
 // Als Double            
 var ss = dou.NummerZuText(true, true);
 label2.Text = ss;
```

## PS:
```cs
var text = $"{_textStrings[num]} und " + s;
```
[String interpolation (C#6.0)](https://github.com/dotnet/roslyn/wiki/New-Language-Features-in-C%23-6#string-interpolation)
