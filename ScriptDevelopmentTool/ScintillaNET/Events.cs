/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
#region Using Directives

using System;

#endregion Using Directives


namespace ScintillaNET
{
    public enum Events : uint
    {
        StyleNeeded = 2000,
        CharAdded = 2001,
        SavePointReached = 2002,
        SavePointLeft = 2003,
        ModifyAttemptRO = 2004,
        SCKey = 2005,
        SCDoubleClick = 2006,
        UpdateUI = 2007,
        Modified = 2008,
        MacroRecord = 2009,
        MarginClick = 2010,
        NeedShown = 2011,
        Painted = 2013,
        UserListSelection = 2014,
        UriDropped = 2015,
        DwellStart = 2016,
        DwellEnd = 2017,
        SCZoom = 2018,
        HotspotClick = 2019,
        HotspotDoubleClick = 2020,
        CallTipClick = 2021,
        AutoCSelection = 2022,
    }
}
