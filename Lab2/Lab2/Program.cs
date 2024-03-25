public abstract class Container
{
    public string SerialNumber { get; protected set; }
    public double LoadMass { get; protected set; }
    public double Height { get; protected set; }
    public double Weight { get; protected set; }
    public double Depth { get; protected set; }
    public double MaxLoad { get; set; }

    public abstract void Load(double mass);
    public abstract void Unload();
}

public interface IHazardNotifier
{
    void NotifyHazard(string message);
}

public class LiquidContainer : Container, IHazardNotifier
{
    public bool IsHazardous { get; set; }

    public override void Load(double mass)
    {
        if (IsHazardous && mass > MaxLoad * 0.5)
        {
            throw new OverfillException("Cannot load more than 50% of capacity for hazardous load.");
        }
        else if (!IsHazardous && mass > MaxLoad * 0.9)
        {
            throw new OverfillException("Cannot load more than 90% of capacity for non-hazardous load.");
        }
        else
        {
            LoadMass = mass;
        }
    }

    public override void Unload()
    {
        LoadMass = 0;
    }

    public void NotifyHazard(string message)
    {
        Console.WriteLine($"Hazard notification for container {SerialNumber}: {message}");
    }
}

public class GasContainer : Container, IHazardNotifier
{
    public double Pressure { get; set; }

    public override void Load(double mass)
    {
        if (mass > MaxLoad)
        {
            throw new OverfillException("Load mass exceeds maximum load capacity.");
        }
        else
        {
            LoadMass = mass;
        }    }

    public override void Unload()
    {
        LoadMass *= 0.05;
    }

    public void NotifyHazard(string message)
    {
        Console.WriteLine($"Hazard notification for container {SerialNumber}: {message}");
    }
}

public class RefrigeratedContainer : Container
{
    public string ProductType { get; set; }
    public double Temperature { get; set; }

    private static readonly Dictionary<string, double> ProductTemperatures = new Dictionary<string, double>
    {
        {"Bananas", 13.3},
        {"Chocolate", 18},
        {"Fish", 2},
        {"Meat", -15},
        {"Ice cream", -18},
        {"Frozen pizza", -30},
        {"Cheese", 7.2},
        {"Sausages", 5},
        {"Butter", 20.5},
        {"Eggs", 19}
    };

    public override void Load(double mass)
    {
        if (mass > MaxLoad)
        {
            throw new OverfillException("Load mass exceeds maximum load capacity.");
        }
        else if (ProductTemperatures[ProductType] > Temperature)
        {
            throw new Exception("The temperature is too low for this product.");
        }
        else
        {
            LoadMass = mass;
        }
    }

    public override void Unload()
    {
        LoadMass = 0;
    }
}

public class Ship
{
    public List<Container> Containers { get; set; }
    public double MaxSpeed { get; set; }
    public int MaxContainerCount { get; set; }
    public double MaxWeight { get; set; }

    public void LoadContainer(Container container)
    {
        if (Containers.Count >= MaxContainerCount)
        {
            throw new Exception("Cannot load more containers. The ship is full.");
        }

        double totalWeight = Containers.Sum(c => c.Weight + c.LoadMass) + container.Weight + container.LoadMass;
        if (totalWeight > MaxWeight * 1000) // MaxWeight is in tons, but weights of containers are in kilograms
        {
            throw new Exception("Cannot load the container. The ship would be overloaded.");
        }

        Containers.Add(container);
        
    }

    public void UnloadContainer(string serialNumber)
    {
        var container = Containers.FirstOrDefault(c => c.SerialNumber == serialNumber);
        if (container == null)
        {
            throw new Exception($"No container with serial number {serialNumber} found on the ship.");
        }

        Containers.Remove(container);
    
    }
}

public class OverfillException : Exception
{
    public OverfillException(string message) : base(message)
    {
    }
}
class Program
{
    static void Main(string[] args)
    {
        // Create containers
        var liquidContainer = new LiquidContainer { IsHazardous = false, MaxLoad = 1000 };
        var gasContainer = new GasContainer { Pressure = 1.0, MaxLoad = 2000 };
        var refrigeratedContainer = new RefrigeratedContainer { ProductType = "Bananas", Temperature = 13.3, MaxLoad = 1500 };

        // Load containers
        try
        {
            liquidContainer.Load(900);
            gasContainer.Load(1900);
            refrigeratedContainer.Load(1400);
        }
        catch (OverfillException ex)
        {
            Console.WriteLine(ex.Message);
        }

        // Create ship
        var ship = new Ship { MaxSpeed = 20, MaxContainerCount = 2, MaxWeight = 3 };

        // Load containers onto ship
        try
        {
            ship.LoadContainer(liquidContainer);
            ship.LoadContainer(gasContainer);
            ship.LoadContainer(refrigeratedContainer);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        // Unload a container from the ship
        try
        {
            ship.UnloadContainer(liquidContainer.SerialNumber);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}
