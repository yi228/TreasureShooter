using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public string gunName; //���̸�
    public float range; //�����Ÿ�
    public float accuracy; //��Ȯ��
    public float fireRate; //����ӵ�
    public float reloadTime; //������ �ӵ�
    public float damage; //������
    public int reloadBulletCount; //�Ѿ� ������ ����
    public int currentBulletCount; //���� ź������ �����ִ� �Ѿ��� ����
    public int maxBulleCount; //�ִ� ���� ���� �Ѿ� ����
    public int carryBulletCount; //���� �������� �Ѿ� ����
    public Animator anim;
}
