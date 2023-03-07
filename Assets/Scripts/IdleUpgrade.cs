using Newtonsoft.Json;
using System;

[Serializable]
public class IdleUpgrade : GenericUpgrade
{
    public BucketNumber basePerSecondValue;

    public IdleUpgrade() : base() { }

    public IdleUpgrade(BucketNumber count, BucketNumber price) : base(count, price) { }

    public IdleUpgrade(BucketNumber count, BucketNumber price, bool unlocked) : base(count, price, unlocked) { }

    public IdleUpgrade(BucketNumber count, BucketNumber price, bool unlocked, BucketNumber basePerSecondValue) : base(count, price, unlocked)
    {
        this.basePerSecondValue = basePerSecondValue;
    }
}