using System;
using Architecture_M;
using UnityEngine;

public class GameDataInstallerPC : GameDataInstallerBase<GameDataPC> {
    
}



[Serializable]
public class GameDataPC : GameDataBase {
    
}



[CreateAssetMenu(menuName = "Architecture_M/Data/Game Data PC")]
public class GameDataPCSO : GameDataBaseSO<GameDataPC>
{

}