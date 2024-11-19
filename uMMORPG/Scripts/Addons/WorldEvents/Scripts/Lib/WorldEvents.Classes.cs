using UnityEngine;

// WORLD EVENT
[System.Serializable]
public partial class WorldEventData
{
    [Tooltip("[Optional] Is the threshold activated when the counter is BELOW or ABOVE it (set to NONE to disable)?")]
    public ThresholdType thresholdType;

    [Tooltip("[Optional] The event counter's threshold value that has be to reached in order to activate.")]
    public int thresholdValue;

    [Tooltip("[Optional] Can the event counter be increased/decreased beyond the threshold or is it capped?")]
    public bool limitToThreshold;

    [Tooltip("[Optional] Does the event counter reset to 0 when it reaches its threshold value? (ignores Limit)")]
    public bool resetOnThreshold;

    [Tooltip("[Optional] When this threshold is reached, stop checking all subsequent thresholds?")]
    public bool stopFurtherThresholdChecks;

    [Tooltip("[Optional] Message all online players when threshold is reached?")]
    public string messageOnThreshold;

    [Tooltip("[Optional] Message only players who actively participated in this event?")]
    public bool messageParticipantsOnly;
}
