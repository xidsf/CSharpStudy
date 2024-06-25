class Item
{
    public int positionX;
    public int positionY;
    public bool isEatable;
    public string itemIcon = "♨";

    public Item(int x, int y, bool isEatable = true)
    {
        positionX = x;
        positionY = y;
        this.isEatable = isEatable;
    }

}