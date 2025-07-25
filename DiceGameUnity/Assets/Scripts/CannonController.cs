using UnityEngine;
using UnityEngine.UI;

public class CannonController : MonoBehaviour
{
    public Transform cannon;
    public LayerMask groundLayer;
    public ParticleSystem particle;
    public Image manaGuage;

    Camera mainCamera;

    public float manaConsumption;
    public float manaGain;
    public float maxMana;
    public float nowMana;

    bool manaLocked;

    private void OnEnable()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (nowMana <= 0)
            manaLocked = true;

        if (manaLocked && nowMana >= maxMana)
            manaLocked = false;

        if (Input.GetMouseButton(0))
        {
            var ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, Mathf.Infinity, groundLayer))
            {
                var targetPosition = hit.point;
                targetPosition.y = cannon.transform.position.y;
                cannon.LookAt(targetPosition);
            }
        }

        if (!manaLocked && Input.GetMouseButton(0))
        {
            particle.Play();

            nowMana -= Time.deltaTime * manaConsumption;
        }
        else
        {
            particle.Stop();

            nowMana += Time.deltaTime * manaGain;
        }

        nowMana = Mathf.Clamp(nowMana, 0f, maxMana);
        manaGuage.fillAmount = nowMana / maxMana;
    }
}
