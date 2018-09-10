namespace Movies.Enums
{
    public enum FileType
    {
        Folder,
        File
    };

    public enum SubtitleType
    {
        SubFile,
        HardCoded
    };

    public enum VolumeState
    {
        Muted,
        Active
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
