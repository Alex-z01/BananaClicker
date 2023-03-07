using Newtonsoft.Json;
using System;

[Serializable]
public class GenericUpgrade
{
    public Stats.StatType stat = Stats.StatType.None;
    public BucketNumber count, price;
    public bool unlocked = false;

    public GenericUpgrade()
    {
        count = BucketNumber.Zero;
        price = BucketNumber.Zero;
    }

    public GenericUpgrade(BucketNumber count, BucketNumber price)
    {
        this.count = count;
        this.price = price;
        unlocked = false;
    }

    public GenericUpgrade(BucketNumber count, BucketNumber price, bool unlocked)
    {
        this.count = count;
        this.price = price;
        this.unlocked = unlocked;
    }

    public override string ToString()
    {
        return $"count: {count}, unlocked: {unlocked}";
    }
}
