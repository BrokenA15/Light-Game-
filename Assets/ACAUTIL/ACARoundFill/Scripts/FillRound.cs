using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class FillRound : MonoBehaviour
{
    float distance;
    public Image ImgFill, ImgStartDot, ImgEndDot;
    public ImgsFillDynamic ImgsFD;
    [SerializeField]
    private bool logInBuild = true;

    private void Awake()
    {

        if (ImgFill == null || ImgStartDot == null || ImgEndDot == null)
        {
            if (logInBuild)
                Debug.LogWarning($"{name}: Falta asignar referencias en FillRound.", this);
        }

        this.distance = Vector3.Distance(this.transform.position, this.ImgStartDot != null ? this.ImgStartDot.transform.position : this.transform.position);

    }



    public void SetFill(float _amount)
    {
        if (ImgFill == null)
        {
            if (logInBuild)
                Debug.LogWarning($"{name}: ImgFill es NULL — revisa en el prefab de lámpara o en la escena.", this);
            return;
        }

        this.ImgFill.fillAmount = _amount;
        this.RefreshAngle();
    }

    void RefreshAngle()
    {
        if (ImgStartDot == null || ImgEndDot == null)
            return;

        float ratio = this.ImgsFD != null ? this.ImgsFD.Factor : this.ImgFill.fillAmount;

        this.ImgStartDot.transform.localPosition = Vector3.zero;
        this.ImgStartDot.transform.rotation = Quaternion.identity;
        this.ImgStartDot.transform.Translate(0, this.distance, 0);

        this.ImgEndDot.transform.localPosition = Vector3.zero;
        this.ImgEndDot.transform.rotation = Quaternion.identity;
        this.ImgEndDot.transform.Rotate(0, 0, -this.GetAngle(ratio), Space.Self);
        this.ImgEndDot.transform.Translate(0, this.distance, 0, Space.Self);
    }

    float GetAngle(float _amount)
    {
        return _amount * 360F;
    }
}
