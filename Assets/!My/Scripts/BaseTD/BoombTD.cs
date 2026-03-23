using UnityEngine;

public class BoombTD : MonoBehaviour
{
    [SerializeField] private bool isActivate = true;
    [SerializeField] private GameObject boombGO;

    public void Boom()
    {
        if (isActivate)
        {
            boombGO.SetActive(true);
            boombGO.transform.parent = null;
        }
        else
        {
            Instantiate(boombGO, transform.position, transform.rotation);
        }

        Destroy(gameObject);
    }
}
