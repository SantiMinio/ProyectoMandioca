﻿using UnityEngine;

public interface IZoneElement
{
    
    [System.Obsolete] void Zone_OnDungeonGenerationFinallized();
    void Zone_OnPlayerEnterInThisRoom(Transform who);
    void Zone_OnPlayerExitInThisRoom();
    void Zone_OnUpdateInThisRoom();
    void Zone_OnPlayerDeath();
}
