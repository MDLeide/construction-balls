using System;

[Serializable]
class ConnectionLink
{
    public ConnectionBlock Creator;
    public ConnectionBlock Other;

    public ConnectionBlock GetOtherSide(ConnectionBlock block)
    {
        if (Creator == block)
            return Other;
        if (Other == block)
            return Creator;

        throw new InvalidOperationException();
    }
}