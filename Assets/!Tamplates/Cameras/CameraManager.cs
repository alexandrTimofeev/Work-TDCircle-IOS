using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using Unity.Cinemachine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private float defaultBlendTime = 0.5f;

    private List<CinemachineCamera> cameras = new List<CinemachineCamera>();
    private Dictionary<int, Action<CinemachineCamera>> followCommands = new Dictionary<int, Action<CinemachineCamera>>();
    private int currentCameraIndex = -1;

    private const int HIGH_PRIORITY = 100;
    private const int LOW_PRIORITY = 1;

    public void Init()
    {
        cameras = FindObjectsByType<CinemachineCamera>(FindObjectsInactive.Include, FindObjectsSortMode.None)
            .OrderBy(cam => ExtractNumber(cam.name))
            .ToList();

        foreach (var cam in cameras)
            cam.Priority = LOW_PRIORITY;

        SwitchToCamera(0, defaultBlendTime);
    }

    void Update()
    {
        foreach (var pair in followCommands)
        {
            if (IsValidCameraIndex(pair.Key))
            {
                pair.Value?.Invoke(cameras[pair.Key]);
            }
        }
    }

    public void SwitchToCamera(int index, float blendTime = 0.5f)
    {
        if (!IsValidCameraIndex(index)) return;

        for (int i = 0; i < cameras.Count; i++)
            cameras[i].Priority = (i == index) ? HIGH_PRIORITY : LOW_PRIORITY;

        var brain = Camera.main?.GetComponent<CinemachineBrain>();
        if (brain != null)
            brain.DefaultBlend.Time = blendTime;

        currentCameraIndex = index;
    }

    public void FollowPosition(Vector3 position) => FollowPosition(position, currentCameraIndex);

    public void FollowPosition(Vector3 position, int index)
    {
        if (!IsValidCameraIndex(index)) return;

        var cam = cameras[index];
        cam.Follow = null;
        cam.LookAt = null;
        cam.transform.position = new Vector3(position.x, position.y, cam.transform.position.z);
    }

    public void FollowObject(Transform target) => FollowObject(target, currentCameraIndex);

    public void FollowObject(Transform target, int index)
    {
        if (!IsValidCameraIndex(index)) return;

        var cam = cameras[index];
        cam.Follow = target;
        cam.LookAt = target;
    }

    public void SetFollowCommand(int index, Action<CinemachineCamera> command)
    {
        if (!IsValidCameraIndex(index)) return;

        followCommands[index] = command;
        followCommands[index] = command;
    }

    public void ClearFollowCommand(int index)
    {
        followCommands.Remove(index);
    }

    private bool IsValidCameraIndex(int index)
    {
        return index >= 0 && index < cameras.Count;
    }

    private int ExtractNumber(string name)
    {
        Match match = Regex.Match(name, @"\((\d+)\)");
        if (match.Success && int.TryParse(match.Groups[1].Value, out int number))
        {
            return number;
        }
        return int.MaxValue;
    }
}