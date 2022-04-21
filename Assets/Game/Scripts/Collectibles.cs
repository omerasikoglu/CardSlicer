public class Collectibles : Singleton<Collectibles>
{
    private int collectibleCount => transform.childCount;

    public int GetCollectibleCount => collectibleCount;

}
