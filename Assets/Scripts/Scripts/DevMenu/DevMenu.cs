using System.Collections.Generic;
using AASave;
using UnityEngine;

public class DevMenu : MonoBehaviour
{
    LevelStreamingManager levelManager;
    CharacterControllerComponent player;
    SC_SubmarineMovement sub;
    SaveSystem saveSystem;
    GameManager gameManager;
    ReloadLoadSave loadLvlManager;


    void Start()
    {
        FindObjects();
    }

    void FindObjects()
    {
        saveSystem = FindAnyObjectByType<SaveSystem>();
        if (saveSystem == null)
            Debug.Log("PlayerSubDeath_Error: No SaveSystem found");

        loadLvlManager = FindAnyObjectByType<ReloadLoadSave>();
        if (loadLvlManager == null)
            Debug.Log("PlayerSubDeath_Error: No ReloadLoadSave found");

        levelManager = FindAnyObjectByType<LevelStreamingManager>();
        if (levelManager == null)
            Debug.Log("PlayerSubDeath_Error: No LevelStreamingManager found");

        player = FindAnyObjectByType<CharacterControllerComponent>();
        if (player == null)
            Debug.Log("PlayerSubDeath_Error: No Player found");

        sub = FindAnyObjectByType<SC_SubmarineMovement>();
        if (sub == null)
            Debug.Log("PlayerSubDeath_Error: No Submarine found");

        // airlock = FindAnyObjectByType<SubmarineAirlock>();
        // if (airlock == null)
        //     Debug.Log("PlayerSubDeath_Error: No Airlock found");
    }
    public void OnDevMenu(string lvlString)
    {
        switch (lvlString)
        {
            case "L_0_OverHole":
                L_0_OverHoleSpawn();
                return;
            case "L_1_Walk":
                L_1_WalkSpawn();
                return;
            case "L_1_LeverRoom":
                L_1_LeverRoom();
                return;
            case "L_2_Start":
                L_2_Start();
                return;
            case "L_2_End":
                L_2_End();
                return;
            case "L_2_EndAfterLoad":
                L_2_EndAfterLoad();
                return;
            case "L_3_Start":
                L_3_Start();
                return;
            case "L_3_Walk":
                L_3_Walk();
                return;
            case "L_3_LeverRoom":
                L_3_LeverRoom();
                return;
            case "L_4_Start":
                L_4_Start();
                return;
            case "L_4_BeforeCity":
                L_4_BeforeCity();
                return;
            case "L_4_City":
                L_4_City();
                return;
            case "L_4_Walk":
                L_4_Walk();
                return;
        }
    }
    public void L_0_OverHoleSpawn()
    {
        var pos = new Vector3(200, 860, 750);
        var rot = new Quaternion(0, -0.60876137f, 0, 0.793353379f);
        List<string> ids = new()
        {
            "Load_L_1",
        };
        StartCoroutine(loadLvlManager.DevMenuLoadLvl(pos, rot, pos, rot, ids.ToArray(), true, "L_0_Dressup", "L_1_Dressup"));
    }
    public void L_1_WalkSpawn()
    {
        var sPos = new Vector3(-45.224823f, 110.349976f, 883.053711f);
        var sRot = new Quaternion(0, -0.721548438f, 0, 0.692364037f);
        var pPos = new Vector3(-51.2999992f, 104.879997f, 882.809998f);
        var pRot = new Quaternion(0, -0.721548438f, 0, 0.692364037f);
        List<string> ids = new()
        {
            "Load_L_2",
            "SCP_L1_01",
            "Dialog_Trigger_L_1_01"
        };
        StartCoroutine(loadLvlManager.DevMenuLoadLvl(sPos, sRot, pPos, pRot, ids.ToArray(), false, "L_1_Dressup", "L_2_Dressup"));
    }

    public void L_1_LeverRoom()
    {
        var sPos = new Vector3(-45.224823f, 110.349976f, 883.053711f);
        var sRot = new Quaternion(0, -0.721548438f, 0, 0.692364037f);
        var pPos = new Vector3(-198.960007f, 105, 872.98999f);
        var pRot = new Quaternion(0, -0.890239656f, 0, 0.455492526f);
        List<string> ids = new()
        {
            "Load_L_2",
            "SCP_L1_01",
            "PCP_L1_01",
            "PCP_L1_02",
            "PCP_L1_03",
            "Dialog_Trigger_L_1_01",
            "Dialog_Trigger_L_1_02"
        };
        StartCoroutine(loadLvlManager.DevMenuLoadLvl(sPos, sRot, pPos, pRot, ids.ToArray(), false, "L_1_Dressup", "L_2_Dressup"));
    }
    public void L_2_Start()
    {
        var pos = new Vector3(-602.478455f, 26.5984001f, 652.320923f);
        var rot = new Quaternion(0, -0.992992401f, 0, 0.1181788f);
        List<string> ids = new()
        {
            "CloseDoor_L_1"
        };
        StartCoroutine(loadLvlManager.DevMenuLoadLvl(pos, rot, pos, rot, ids.ToArray(), true, "L_1_Dressup", "L_2_Dressup"));
    }
    public void L_2_End()
    {
        var pos = new Vector3(-295, -49, -137);
        var rot = new Quaternion(0, -0.978147745f, 0, -0.207911476f);
        List<string> ids = new()
        {
            "CloseDoor_L_1"
        };
        StartCoroutine(loadLvlManager.DevMenuLoadLvl(pos, rot, pos, rot, ids.ToArray(), true, "L_1_Dressup", "L_2_Dressup"));
    }
    public void L_2_EndAfterLoad()
    {
        var pos = new Vector3(-76.402771f, -164.479996f, -523.5896f);
        var rot = new Quaternion(0, -0.965456784f, 0, -0.260563433f);
        List<string> ids = new()
        {
            "Load_L_3"
        };
        StartCoroutine(loadLvlManager.DevMenuLoadLvl(pos, rot, pos, rot, ids.ToArray(), true, "L_2_Dressup", "L_3_Dressup"));
    }
    public void L_3_Start()
    {
        var pos = new Vector3(69.4900818f, -145.5f, -928.319702f);
        var rot = new Quaternion(0, -0.957162976f, 0, 0.289549828f);
        List<string> ids = new()
        {
            "Dialog_Trigger_L_3_01"
        };
        StartCoroutine(loadLvlManager.DevMenuLoadLvl(pos, rot, pos, rot, ids.ToArray(), true, "L_2_Dressup", "L_3_Dressup"));
    }
    public void L_3_Walk()
    {
        var sPos = new Vector3(-261.063721f, -210.5858f, -1273.72498f);
        var sRot = new Quaternion(0, -0.980002463f, 0, 0.198985443f);
        var pPos = new Vector3(-265.642059f, -218.102798f, -1280.91907f);
        var pRot = new Quaternion(0, -0.976110637f, 0, 0.2172741f);
        List<string> ids = new()
        {
            "Load_L_4",
            "Dialog_Trigger_L_3_01",
            "Dialog_Trigger_L_3_02",
            "Dialog_Trigger_L_3_03",
            "Dialog_Trigger_L_3_04"
        };
        StartCoroutine(loadLvlManager.DevMenuLoadLvl(sPos, sRot, pPos, pRot, ids.ToArray(), false, "L_3_Dressup", "L4_Dressup"));
    }
    public void L_3_LeverRoom()
    {
        var sPos = new Vector3(-261.063721f, -210.5858f, -1273.72498f);
        var sRot = new Quaternion(0, -0.980002463f, 0, 0.198985443f);
        var pPos = new Vector3(-219.459457f, -238.046799f, -1398.35095f);
        var pRot = new Quaternion(0, -0.903806806f, 0, -0.427940786f);
        List<string> ids = new()
        {
            "Load_L_4",
            "Dialog_Trigger_L_3_01",
            "Dialog_Trigger_L_3_02",
            "Dialog_Trigger_L_3_03",
            "Dialog_Trigger_L_3_04"

            //Check for one time triggers before you pull the lever
        };
        StartCoroutine(loadLvlManager.DevMenuLoadLvl(sPos, sRot, pPos, pRot, ids.ToArray(), false, "L_3_Dressup", "L4_Dressup"));
    }
    public void L_4_Start()
    {
        var pos = new Vector3(-92.578598f, -223.5f, -1560.84888f);
        var rot = new Quaternion(0, 0.97982198f, 0, 0.199872479f);
        List<string> ids = new()
        {
            "CloseDoor_L_3"

            //Check for one time triggers
        };
        StartCoroutine(loadLvlManager.DevMenuLoadLvl(pos, rot, pos, rot, ids.ToArray(), true, "L_3_Dressup", "L4_Dressup"));
    }
    public void L_4_BeforeCity()
    {
        var pos = new Vector3(356.671082f, -209.088867f, -2313.68091f);
        var rot = new Quaternion(0, 0.499999225f, 0, 0.866025865f);
        List<string> ids = new()
        {
            "CloseDoor_L_3"

            //Check for one time triggers before you enter the city
        };
        StartCoroutine(loadLvlManager.DevMenuLoadLvl(pos, rot, pos, rot, ids.ToArray(), true, "L_3_Dressup", "L4_Dressup"));
    }
    public void L_4_City()
    {
        var pos = new Vector3(472.929993f, -215.929993f, -2486.56006f);
        var rot = new Quaternion(0, 0.973172903f, 0, 0.230075195f);
        List<string> ids = new()
        {
            "CloseDoor_L_3"

            //Check for one time triggers before you enter the city
        };
        StartCoroutine(loadLvlManager.DevMenuLoadLvl(pos, rot, pos, rot, ids.ToArray(), true, "L_3_Dressup", "L4_Dressup"));
    }
    public void L_4_Walk()
    {
        var sPos = new Vector3(554.719971f, -184.389999f, -2653.87012f);
        var sRot = new Quaternion(0, -0.973751366f, 0, -0.227614298f);
        var pPos = new Vector3(558.417297f, -191.783005f, -2661.25659f);
        var pRot = new Quaternion(0, 0.954505384f, 0, 0.298193663f);
        List<string> ids = new()
        {
            //Check for one time triggers before end walk
        };
        StartCoroutine(loadLvlManager.DevMenuLoadLvl(sPos, sRot, pPos, pRot, ids.ToArray(), false, "L_3_Dressup", "L4_Dressup"));
    }
}
