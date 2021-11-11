using System;

class NewBuildingEventArgs : EventArgs
{
    public NewBuildingEventArgs(Building building)
    {
        NewBuilding = building;
    }

    public Building NewBuilding { get; }
}