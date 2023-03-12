public class StatUpgrade : GenericUpgrade
{
    public BucketNumber baseValueModifier;

    public StatUpgrade() : base() { }

    public StatUpgrade(BucketNumber count, BucketNumber price) : base(count, price) { }

    public StatUpgrade(BucketNumber count, BucketNumber price, bool unlocked) : base(count, price, unlocked) { }

    public StatUpgrade(BucketNumber count, BucketNumber price, bool unlocked, Stats.StatType stat,  BucketNumber baseValueModifier) : base(count, price, unlocked)
    {
        this.count = count;
        this.stat = stat;
        this.baseValueModifier = baseValueModifier;
    }
}
