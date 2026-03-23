using System.Collections;
using UnityEngine;

namespace SGames
{
    public class OpenLink : MonoBehaviour
    {
        [SerializeField] protected string _link = "https://linkojourney.com/privacy-policy.html";


        public virtual void OpenLinkInBrowser()
        {
            BrowserUtils.OpenUrl(_link);
        }
    }
}