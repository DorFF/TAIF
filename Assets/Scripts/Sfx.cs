using UnityEngine;

public static class Sfx
{
    // mechanic sound //
    private static AudioClip click = Resources.Load("Sounds/Interface") as AudioClip;
    private static AudioClip gear = Resources.Load("Sounds/click") as AudioClip;
    private static AudioClip clickgal = Resources.Load("Sounds/Push") as AudioClip;

    public static void Click()
    {
        AudioSource.PlayClipAtPoint(click, Camera.main.transform.position);
    }

    public static void GearClick()
    {
        AudioSource.PlayClipAtPoint(gear, Camera.main.transform.position);
    }

    public static void GalleryClick()
    {
        AudioSource.PlayClipAtPoint(clickgal, Camera.main.transform.position);
    }

    
}
