using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class GunController : MonoBehaviour
{
    //Ȱ��ȭ ����
    public static bool isActivate = false;
    //���� ������ ��
    [SerializeField] private Gun currentGun;
    //ȿ����
    public AudioSource gunAudioSource;
    //���� �ӵ� 
    private float currentFireRate;
    //���� ����
    public bool isReload = false;

    //������ �浹 ���� �޾ƿ�
    private RaycastHit hitInfo;

    [SerializeField] private Camera theCam;
    private Crosshair crosshair;

    //�ǰ� ����Ʈ
    [SerializeField] private GameObject hit_effect_prefab;

    [SerializeField] private LayerMask layerMask;

    [SerializeField] private Transform bulletStartPoint;

    public Crosshair Cross { get { return crosshair; } set { crosshair = value; } }

    void Update()
    {
        if (!GetComponent<PlayerController>().dead && !GameManager.instance.menuOn && !GameManager.gameEnd)
        {
            GunFireRateCalc();
            TryFire();
            TryReload();
        }
    }
    //����ӵ� ����
    private void GunFireRateCalc()
    {
        if (currentFireRate > 0)
            currentFireRate -= Time.deltaTime; // 1���� ���� = 60����1, �ᱹ 1�ʿ� 1 �� ���ҽ�Ŵ
    }
    //�߻�õ�
    private void TryFire()
    {
        if (Input.GetButton("Fire1") && currentFireRate <= 0 && isReload == false)
            Fire();
    }
    //�߻� �� ���
    private void Fire()
    {
        if (!isReload)
        {
            if (currentGun.currentBulletCount > 0)
                Shoot();
            else
                StartCoroutine(ReloadCoroutine());
        }
    }
    //������
    IEnumerator ReloadCoroutine()
    {
        if (currentGun.carryBulletCount > 0)
        {
            isReload = true;
            currentGun.anim.SetTrigger("Reload");
            SoundManager.instance.PlayClip(gunAudioSource, SoundManager.instance.audioClips["Reload"]);
            currentGun.carryBulletCount += currentGun.currentBulletCount; //�Ѿ� ����ä�� �����ϸ� �����Ѿ� ����
            currentGun.currentBulletCount = 0;
            yield return new WaitForSeconds(currentGun.reloadTime);
            if (currentGun.carryBulletCount >= currentGun.reloadBulletCount)
            {
                currentGun.currentBulletCount = currentGun.reloadBulletCount;
                currentGun.carryBulletCount -= currentGun.reloadBulletCount;
            }
            else
            {
                currentGun.currentBulletCount = currentGun.carryBulletCount;
                currentGun.carryBulletCount = 0;
            }
            isReload = false;
        }
        else
            Debug.Log("�Ѿ� ����");
    }
    //������ �õ�
    private void TryReload()
    {
        if (Input.GetKeyDown(KeyCode.R) && !isReload && currentGun.currentBulletCount < currentGun.reloadBulletCount)
            StartCoroutine(ReloadCoroutine());
    }
    public void CancelReload()
    {
        if (isReload)
        {
            StopAllCoroutines();
            isReload = false;
        }
    }
    //�߻���
    private void Shoot()
    {
        currentGun.currentBulletCount--;
        currentGun.anim.SetTrigger("Shoot");
        SoundManager.instance.PlayClip(gunAudioSource, SoundManager.instance.audioClips["Fire"]);
        GetComponent<PhotonView>().RPC("PlayGunSound", RpcTarget.All, "Fire");
        crosshair.FireAnimation();
        currentFireRate = currentGun.fireRate; //����ӵ� ����
        Hit();
        //�ѱ�ݵ� �ڷ�ƾ ����
        StopAllCoroutines();
    }
    private void Hit()
    {
        if (Physics.Raycast(bulletStartPoint.position, theCam.transform.forward +
            new Vector3(Random.Range(-crosshair.GetAccuracy() - currentGun.accuracy, crosshair.GetAccuracy() + currentGun.accuracy),
            Random.Range(-crosshair.GetAccuracy() - currentGun.accuracy, crosshair.GetAccuracy() + currentGun.accuracy), 0),
            out hitInfo, currentGun.range, layerMask))
        {
            var effect = Instantiate(hit_effect_prefab, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
            Destroy(effect, 2f);
            //if (hitInfo.transform.gameObject.tag == "Player")
            //{
            //    hitInfo.transform.GetComponent<Player>().Hp-=currentGun.damage;
            //    if (hitInfo.transform.GetComponent<Player>().Hp < 0)
            //        hitInfo.transform.GetComponent<Player>().Hp = 0;
            //    print("���� ������ HP:" + hitInfo.transform.GetComponent<Player>().Hp);
            //}
            //else
            if (hitInfo.transform.gameObject.GetComponent<Player>())
            {
                hitInfo.transform.gameObject.GetComponent<PhotonView>().RPC("DecreaseHP", RpcTarget.All, currentGun.damage, GetComponent<Player>().id);
                SoundManager.instance.PlayClip(gunAudioSource, SoundManager.instance.audioClips["Fire"]);
                SoundManager.instance.PlayClip(GetComponent<PlayerController>().playerAudioSource, SoundManager.instance.audioClips["Damage"]);
            }
            //if (hitInfo.transform.gameObject.tag == "Monster")
            //{
            //    hitInfo.transform.GetComponent<SlimeController>().Hp-=(int)currentGun.damage;
            //    if (hitInfo.transform.GetComponent<SlimeController>().Hp < 0)
            //    {
            //        Debug.Log("������ óġ");
            //        hitInfo.transform.GetComponent<SlimeController>().Catcher = transform.GetComponent<PlayerController>();
            //        hitInfo.transform.GetComponent<SlimeController>().SetBuff();
            //        Destroy(hitInfo.transform.gameObject);
            //    }
            //}
            if (hitInfo.transform.gameObject.GetComponent<SlimeController>())
            {
                hitInfo.transform.gameObject.GetComponent<PhotonView>().RPC("DecreaseHP", RpcTarget.All, currentGun.damage);
                SoundManager.instance.PlayClip(gunAudioSource, SoundManager.instance.audioClips["Fire"]);
                SoundManager.instance.PlayClip(GetComponent<PlayerController>().playerAudioSource, SoundManager.instance.audioClips["Damage"]);
                if (hitInfo.transform.gameObject.GetComponent<SlimeController>().Hp <= 0)
                {
                    hitInfo.transform.GetComponent<SlimeController>().Catcher = transform.GetComponent<PlayerController>();
                    hitInfo.transform.GetComponent<SlimeController>().SetBuff();
                    SoundManager.instance.PlayClip(GetComponent<PlayerController>().playerAudioSource, SoundManager.instance.audioClips["MonsterKill"]);
                    Destroy(hitInfo.transform.gameObject);
                }
            }
        }
    }
}