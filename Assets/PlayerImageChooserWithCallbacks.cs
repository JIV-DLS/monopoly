
public class PlayerImageChooserWithCallbacks : PlayerImageChooser
{
    public ConnectedToRoom connectedToRoom;
    
    protected override void SetCurrentIndex(int chosenImagesCount)
    {
        base.SetCurrentIndex(chosenImagesCount);
        connectedToRoom.SetCurrentImageIndex(chosenImagesCount);
    }
}
