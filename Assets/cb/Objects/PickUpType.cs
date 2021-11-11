using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


enum PickUpType
{
    Static = -100,
    Default = 0,
    BlueBall = 1,
    RedBall = 2,
    YellowBall = 3,

    BlueBuildingBlock = 101,
    RedBuildingBlock = 102,
    YellowBuildingBlock = 103,

    OpenBlock = 201,
    PipeBlock = 202,
    AccelBlock = 203,
    TubeBlock = 204,

    StraightHalfPipe = 301,
    InclinedHalfPipe = 302,
}