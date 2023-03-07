using Newtonsoft.Json;
using System;

[Serializable]
public class ActiveUpgrade : GenericUpgrade
{
    public BucketNumber basePerClickValue;

    public ActiveUpgrade() : base() { }

    public ActiveUpgrade(BucketNumber count, BucketNumber price) : base(count, price) { }

    public ActiveUpgrade(BucketNumber count, BucketNumber price, bool unlocked) : base(count, price, unlocked) { }

    public ActiveUpgrade(BucketNumber count, BucketNumber price, bool unlocked, BucketNumber basePerClickValue) : base(count, price, unlocked)
    {
        this.basePerClickValue = basePerClickValue;
    }
}