public class Valve
{
    // FIELD: a variable that belongs to each object. private = only this class can touch it.
    private float _pressure;

    // PROPERTY: like a field, but with built-in get (read) and set (write) rules.
    public string Name { get; set; }              // anyone can read and change
    public bool IsOpen { get; private set; }      // anyone can read; only this class can change

    // PROPERTY WITH A RULE: pressure can never go negative.
    public float Pressure
    {
        get { return _pressure; }
        set { _pressure = value < 0 ? 0 : value; }   // 'value' = whatever is being assigned
    }

    // CONSTRUCTOR: runs once when an object is created. Same name as the class, no return type.
    public Valve(string name)
    {
        Name = name;
        IsOpen = true;
    }

    // METHODS: things a valve can do.
    public void Close()
    {
        IsOpen = false;
        Console.WriteLine($"{Name} closed");
    }
}