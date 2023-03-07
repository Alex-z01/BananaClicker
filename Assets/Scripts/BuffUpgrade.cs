using System;
using System.Collections.Generic;

[Serializable]
public class BuffUpgrade : GenericUpgrade
{
    public List<string> buffTargets = new List<string>();
    public BucketNumber buffValue;

    public BuffUpgrade() : base() { }

    public BuffUpgrade(BucketNumber count, BucketNumber price) : base(count, price) { }

    public BuffUpgrade(BucketNumber count, BucketNumber price, bool unlocked) : base(count, price, unlocked) { }

    public BuffUpgrade(BucketNumber count, BucketNumber price, bool unlocked, BucketNumber buffValue, List<string> buffTargets) : base(count, price, unlocked)
    {
        this.buffValue = buffValue;
        this.buffTargets = buffTargets;
    }

}