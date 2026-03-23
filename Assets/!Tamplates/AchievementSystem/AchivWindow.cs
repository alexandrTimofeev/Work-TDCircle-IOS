using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class AchivWindow : WindowUI
{
    [SerializeField] private AchivVisual achivVisualPref;
    private List<AchivVisual> achivVisuals = new List<AchivVisual>();
    private List<AchivInfo> achivsFirstView = new List<AchivInfo>();

    [SerializeField] private Transform container;

    [Space]
    [SerializeField] private AchivInfoWindowUI InfoWindowUI;
    [SerializeField] private AudioSource source;

    public override void Open()
    {
        base.Open();

        UpdateAchivContainer();
        FirstViewAll();
    }

    private void UpdateAchivContainer()
    {
        ClearAchivVisuals();

        AchivInfo[] achivInfos = AchieviementSystem.GetAllAchivInfo();
        StartCoroutine(UpdateSkinContainerCoroutine(achivInfos));
    }


    private IEnumerator UpdateSkinContainerCoroutine(AchivInfo[] achivInfos)
    {
        foreach (var achiv in achivInfos)
        {
            var achivVisual = Instantiate(achivVisualPref, container);
            achivVisual.Init(achiv);
            achivVisual.OnClickOpen += OpenAchivInfoWindow;
            achivVisuals.Add(achivVisual);

            if (AchieviementSystem.IsUnlockAchiv(achiv.ID) && IsFirstView(achiv.ID))
                AddListFirstView(achiv);
            yield return new WaitForSeconds(0.15f);
        }
    }

    private void AddListFirstView(AchivInfo achiv)
    {
        achivsFirstView.Add(achiv);
    }

    private bool IsFirstView(string id)
    {
        return PlayerPrefs.GetInt($"AchivWindowFirstView_{id}", 0) == 0;
    }

    private void ClearAchivVisuals()
    {
        achivVisuals.ForEach((x) => Destroy(x.gameObject));
        achivVisuals.Clear();
    }

    private void FirstViewAll()
    {
        StartCoroutine(FirstViewAllCotoutine());
    }

    private IEnumerator FirstViewAllCotoutine ()
    {
        Debug.Log($"FirstViewAllCotoutine achivsFirstView.Count {achivsFirstView.Count}");
        for (int i = 0; i < achivsFirstView.Count; i++)
        {
            yield return StartCoroutine(FirstViewCotoutine(achivsFirstView[i]));
            yield return new WaitForEndOfFrame();
        }
        achivsFirstView.Clear();
    }

    private IEnumerator FirstViewCotoutine (AchivInfo achiv)
    {
        PlayerPrefs.SetInt($"AchivWindowFirstView_{achiv.ID}", 1);
        OpenAchivInfoWindow(achiv);
        source.Play();
        yield return new WaitWhile(() => InfoWindowUI.IsOpen);
        yield break;
    }

    public void OpenAchivInfoWindow (AchivInfo achiv)
    {
        InfoWindowUI.Init(achiv);
        InfoWindowUI.Open();
    }

    /*public void CloseAchivInfoWindow ()
    {

    }*/
}