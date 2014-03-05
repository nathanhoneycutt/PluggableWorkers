#Project Description
PluggableWorkers is a framework for building one-off, self-contained worker classes in your .NET language of choice.

#Main Features
PluggableWorkers allows developers to write self-contained classes that are highly configurable but don't have all of the ceremony of larger projects around them.

* Each worker's making entry point is an Invoke() method that is called by the Host.
* Each worker defines a nested Settings class, and those settings are automatically loaded from the XML configuration file.

PluggableWorkers leverages StructureMap heavily so that developers don't have to worry about the dependencies of workers.  By default, log4net is baked into the Host so that workers can request an ILog interface and get an appropriate logger.

#Examples

```csharp
public class SampleWorker : IDoWork
{
	private readonly Settings _settings;
	
	public SampleWorker(Settings settings)
	{
		_settings = settings;
	}
	
	public bool Invoke()
	{
		Console.WriteLine("Whatever you want to do to, do it here.");
		return true;
	}
	
	public class Settings
	{
		public string SourceFilename { get; set; }
		public int[] SomeNumbers { get; set; }
	}
}

```

```xml
<pluggableWorkers>
	<workers>
		<worker name="Sample" type="MyApplication.SampleWorker,MyApplication">
			<SourceFilename>C:\temp\Whatever.txt</SourceFilename>
			<SomeNumbers>1;3;5;7;9</SomeNumbers>
		</worker>
	</workers>
</pluggableWorkers>

```
#License
This project is licensed under the Microsoft Public License (Ms-PL).  Please see LICENSE.TXT for terms.
