namespace Recorder.Models
{
    public class AudioChannelsAndStreams
    {
        public string DisplayName { get; set; }
        public int Channels { get; set; }
        public int Streams { get; set; }
        public int Id { get; set; }

        public bool Is4Channels => Channels == 4;
        
    }
}
