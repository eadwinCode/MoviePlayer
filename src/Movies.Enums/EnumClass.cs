namespace Movies.Enums
{
    public enum GroupCatergory
    {
        Grouped,
        Child
    };

    public enum Channel
    {
        Stereo,
        Mono
    };


    public enum SubtitleType
    {
        SubFile,
        HardCoded
    };

    public enum SCREENSETTINGS
    {
        Normal,
        Fullscreen
    };

   
    public enum VolumeState : int
    {
        Active,
        Muted
    };

    public enum DestroyTime
    {
        Short ,
        Long 
    };

    public enum SortType
    {
        Date,
        Name,
        Extension
    };

    public enum ViewType
    {
        Small,
        Large
    };

    public enum RepeatMode
    {
        Repeat,
        RepeatOnce,
        NoRepeat
    };

    public enum RadioSort
    {
        Favorite,
        TopRated,
        Personal
    };

    public enum MovieMediaState
    {
        NothingSpecial = 0,
        Opening,
        Buffering,
        Playing,
        Paused,
        Stopped,
        Ended,
        Error
    };
    
}
