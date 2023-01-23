# Contributing to GyroShell
So, word came around that you want to contribute to GyroShell. Awesome! Before you do, let's make sure you're up to speed on all the rules.

# 1. Naming your PRs and Commits
To make it easier for us to verify your PR and review it, we request that you please name your commits and PRs well. Here's a few things to make sure you fit this criteria:
```
1. Make your names descriptive and easy to read. E.G: "Add window management system for DefaultTaskbar."
2. Keep the names short. Don't name your commit "Adding window management system into the DefaultTaskbar with hovering support."
3. Try and use proper grammar and avoid typos. Don't name the commit "add wifdow management in defaurttaskbar"
4. Keep it clean. Don't use swear words or other vulgar language as it's not needed.
```
That's it for the naming guidelines!

# 2. Testing
Before you submit your PR, thoroughly test your code and run it through as many different scenarios as possible. This is to avoid buggy or slow code in GyroShell.

# 3. Code Quality
Leading from the previous rule, make sure your code is high quality and clean. Always use the best method possible in your code, to make it as efficient and clean as possible. I'll provide some examples below to guide you in the right direction.
`Prompt #1: Create a simple method to detect the current system battery level and then return a value.`

**Bad Code:**
```cs
// This is just a partial example.
var aggBattery = Battery.AggregateBattery;
var report = aggBattery.GetReport();
string charging = report.Status.ToString();
double fullCharge = Convert.ToDouble(report.FullChargeCapacityInMilliwattHours);
double currentCharge = Convert.ToDouble(report.RemainingCapacityInMilliwattHours);
double battLevel = Math.Ceiling((currentCharge / fullCharge) * 100);
if (charging == "Charging" || charging == "Idle")
{
    if (battLevel > 95)
    {
        BattStatus.Text = "\uEBB5";
    }
    else if (battLevel >= 90)
    {
        BattStatus.Text = "\uEBB4";
    }
    // etc. An if else statement to do all the battery calculations would be slow, and would take up 100+ lines of code.
}
else
{
    if (battLevel > 95)
    {
        BattStatus.Text = "\uEBAA";
    }
    else if (battLevel >= 90)
    {
        BattStatus.Text = "\uEBA9";
    }
    // etc. Again, using an if statement for both conditions is slow and takes up a lot of space.
}
```

**Good Code:**
```cs
// This is just a partial example.
string[] batteryIconsCharge = { "\uEBAE", "\uEBAC", "\uEBAD", "\uEBAE", "\uEBAF", "\uEBB0", "\uEBB1", "\uEBB2", "\uEBB3", "\uEBB4", "\uEBB5" };
string[] batteryIcons = { "\uEBA0", "\uEBA1", "\uEBA2", "\uEBA3", "\uEBA4", "\uEBA5", "\uEBA6", "\uEBA7", "\uEBA8", "\uEBA9", "\uEBAA" };
var aggBattery = Battery.AggregateBattery;
var report = aggBattery.GetReport();
string charging = report.Status.ToString();
double fullCharge = Convert.ToDouble(report.FullChargeCapacityInMilliwattHours);
double currentCharge = Convert.ToDouble(report.RemainingCapacityInMilliwattHours);
double battLevel = Math.Ceiling((currentCharge / fullCharge) * 100);
if (charging == "Charging" || charging == "Idle")
{
    int indexCharge = (int)Math.Floor(battLevel / 10);
    BattStatus.Text = batteryIconsCharge[indexCharge];
}
else
{
    int indexDischarge = (int)Math.Floor(battLevel / 10);
    BattStatus.Text = batteryIcons[indexDischarge];
}
// This entire block of code can set the battery level, while being faster and only taking up 17 lines of space.
```

Let's get one more example in here. This next prompt is based on #1.
`Prompt #2: Update the battery level text whenever it changes.`

**Bad Code:**
```cs
private void InternetUpdate()
{
    DispatcherTimer battUpdate = new DispatcherTimer();
    battUpdate.Tick += BattUpdateMethod;
    battUpdate.Interval = new TimeSpan(10000000);
    batttUpdate.Start();
}
// This code is very inefficient because it runs a timer and just checks, even if the battery level hasn't changed at all. Always avoid using timers.
```

**Good Code:**
```cs
// Constructor
{
    Battery.AggregateBattery.ReportUpdated += AggregateBattery_ReportUpdated;
}

private void AggregateBattery_ReportUpdated(Battery sender, object args)
{
    if (reportRequested)
    {
        DispatcherQueue.TryEnqueue((Microsoft.UI.Dispatching.DispatcherQueuePriority)CoreDispatcherPriority.Normal, () =>
        {
            AggregateBattery();
        });
    }
}
// This code is much more efficient because it subsribes to an event and only fires when the battery charge level is updated, saving CPU cycles, and decresing the overall CPU usage of GyroShell.
```
You should always use events when avalible. If not, you may need to use a timer. But before writing a PR, you have to do your research and atleast look for an event in the API. We will check this during code review.

# 4. Naming your code and comments
In programming, people generally follow certain naming schemes for the variables, methods, fields, etc. This concept applies in GyroShell. We follow the PascalCase and camelCase naming schemes. Let me show you the naming schemes we use:
```cs
// Good names for fields or variables (camelCase):
string someString = "Hello, world!";
int someInteger = 46;
var someVariable = report.Value;
double percent = 98.65;

// Good names for methods (PascalCase):
private void SomePrivateMethod(int someArg, string anotherArg)
{
    // Your code here
}

public void SomePublicMethod(int someArg, string anotherArg)
{
    // Your code here
}
```

Here's a few examples of bad names. This includes any other type cases, along with non English names, typos, etc. If your code has names in it that are words from another language, like "cuerda" instead of "string", then we won't accept the PR. Also, make sure to keep your names short and concise, but don't make them acronyms like the `double` field shown below.
```cs
// Bad names for fiels or variables (any other type case that isn't Pascal or camel):
string some_string = "Hello, world!";
int SOMEinteger = 46;
var somevariable = report.Value;
double PRCNT = 98.65;

// Bad names for methods
private void someprivateMETHOD(int somearg, string ANOTHERARG)
{
    // Your code here
}

public void some_public_method(int some-arg, string ANTHRARG)
{
    // Your code here
}
```

Lastly for this rule, don't overuse comments. Comment lines where things are more complex, but not every single method or line. Example:
```cs
// Don't do this!

// some method to parse an rss feed
public void ParseRSS()
{
    // create feed
    SyndicationFeed feed = new SyndicationFeed;
    // load data
    list.DataSource = feed.Items;
    // print done to debug
    Debug.WriteLine("Done!");
}
```

# 5. Don't make a mess in the codebase
If you're going to make a PR, make it for one purpose. Don't just make all of your changes in one PR, like "Added window management, sound api support, settings support". Just make a few PRs if you have multiple features to add. And keep the codebase clean. Only keep the necessary code that you need for the feature. So if there's any extra test code left behind, make sure to remove it before requesting a code review. Lastly, just make sure your submission has a purpose. If it's something simple like "change the color of the taskbar", we probably won't merge it.
