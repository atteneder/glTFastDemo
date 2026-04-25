// SPDX-FileCopyrightText: 2026 Andreas Atteneder
// SPDX-License-Identifier: Apache-2.0

using System;

public struct StopWatchState : IEquatable<StopWatchState>
{
    public float duration;
    public int frameCount;
    public float averageFrameTime;
    public float maxFrameTime;
    public float minFrameTime;

    public override bool Equals(object obj)
    {
        if (obj is StopWatchState other)
        {
            return Equals(other);
        }

        return false;
    }

    public bool Equals(StopWatchState other)
    {
        return duration.Equals(other.duration)
               && frameCount == other.frameCount
               && averageFrameTime.Equals(other.averageFrameTime)
               && maxFrameTime.Equals(other.maxFrameTime)
               && minFrameTime.Equals(other.minFrameTime);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(duration, frameCount, averageFrameTime, maxFrameTime, minFrameTime);
    }
}