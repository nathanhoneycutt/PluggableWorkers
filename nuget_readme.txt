----------------
PluggableWorkers
----------------
Thanks for installing PluggableWorkers, a framework for building one-off,
self-contained worker classes in your .NET language of choice.

If you need help, feel free to contact me on Twitter @nlhoneycutt.


---------------
Getting Started
---------------
PluggableWorkers is by no means a complicated framework, but there are
some rules you have to follow for success.  You'll need an actual
worker, you'll need some way to run that worker, and you'll need a
configuration file.


-----------------
Your First Worker
-----------------
Let's look at a very simple sample worker.

    public class SampleWorker : IDoWork
    {
        private readonly Settings _settings;
        
        public SampleWorker(Settings settings)
        {
            _settings = settings;
        }
        
        public bool Invoke()
        {
            Console.WriteLine("Hello, {0}", _settings.YourName);
            Array.Sort(_settings.SomeNumbers);
            Console.WriteLine("\t{0}", String.Join("," _settings.SomeNumbers));
            
            return true;
        }
        
        public class Settings
        {
            public string YourName { get; set; }
            public int[] SomeNumbers { get; set; }
        }
    }

That's all there is to it.  A worker is something that implements
the IDoWork interface and contains a nested public Settings class.


-----------------------
Configuring Your Worker
-----------------------
You will need to create a PluggableWorkerHost.config file to sit 
alongside whatever application is going to invoke your workers.
This NuGet package includes a sample 
"PluggableWorkerHost.config.sample" in your project. To get started
quickly, remove the ".sample", and alter the file properties as 
follows:
    Build Action:  None
    Copy to Output Directory: Copy if newer
	
This sample contains the configuration for a sample worker that is
built into the PluggableWorkers library.  To configure the sample
worker above, you'll want to alter this sample file to look like
the following:

    <?xml version="1.0" encoding="utf-8" ?>
    <pluggableWorkers>
        <workers>
            <worker type="YourProject.NewTestworker,YourProject"
			        name="NameYouAssign">
                <YourName>John</YourName>
                <SomeNumbers>2;4;6;8;10;1;3;5;7;9</SomeNumbers>
            </worker>
        </workers>
    </pluggableWorkers>

Note that you must specify the fully-qualified type for your worker,
meaning the full class name (including namespaces), a comma, and then
the name of the assembly containing the type definition.


----------
Running It
----------
The quickest path to running your worker configuration is to create a console
application that references the PluggableWorkers package.  In the main method,
do the following:

    PluggableWorkerHost.CreateHost("PluggableWorkerHost.config").Invoke();

That's it! This will spin up PluggableWorkers, read your configuration file,
configure the worker(s) inside, and run them in order.

Of course, there is far more that you can do with PluggableWorkers. Check out
my blog (http://nathanhoneycutt.net/blog) for more information.
