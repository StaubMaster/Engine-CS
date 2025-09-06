using Engine3D.Abstract2D;
using Engine3D.Graphics.Display;

namespace Engine3D.Graphics
{
    /*
     *  Anchor and Position (Normal Position and Pixel Position) could be used generally for UI Elements
     *  Offset also ? for something like Inventory ? yes
     *  but Width Height and Padding would also work
     *  should Forms have their own thing ?
     *      no, Form looks for dist from both sides (sometimes)
     *  UI Grid Elements ?
     */
    public struct TextSize
    {
        public UIGridSize Size;
        public float Thick;

        public TextSize(float height, float thick, float padding)
        {
            Size = new UIGridSize(new Point2D(height * 0.5f, height), padding);
            Thick = thick;
        }
    }
}
