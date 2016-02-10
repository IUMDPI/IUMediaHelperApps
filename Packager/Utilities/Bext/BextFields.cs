using Packager.Attributes;

namespace Packager.Utilities.Bext
{
    public enum BextFields
    {
        [FFMPEGArgument("description")] Description,
        [FFMPEGArgument("originator")] Originator,
        [FFMPEGArgument("originator_reference")] OriginatorReference,
        [FFMPEGArgument("origination_date")] OriginationDate,
        [FFMPEGArgument("origination_time")] OriginationTime,
        [FFMPEGArgument("time_reference")] TimeReference,
        [FFMPEGArgument("coding_history")] History,
        [FFMPEGArgument("umid")] UMID,
        IARL,
        IART,
        ICMS,
        ICMT,
        ICOP,
        ICRD,
        IENG,
        IGNR,
        IKEY,
        IMED,
        INAM,
        IPRD,
        ISBJ,
        ISFT,
        ISRC,
        ISRF,
        ITCH
    }
}