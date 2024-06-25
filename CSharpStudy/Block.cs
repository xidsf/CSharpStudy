using System.Numerics;

class Block
{
    public int positionX = 1;
    public int positionY = 1;
    public bool isInPlayer = false;
    public bool isHidden = false;

    public string Icon = "□";
    public string playerInIcon = "▣";

    public Block(int x, int y, bool isHidden = false)
    {
        positionX = x;
        positionY = y;
        this.isHidden = isHidden;
        isInPlayer = false;
    }
}