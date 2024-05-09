using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RandomRoomName : MonoBehaviour
{
    private TMP_InputField text;

    void Start()
    {
        text = GetComponent<TMP_InputField>();
        int num = Random.Range(0, 10);
        text.text = SetRoomName(num);
        RoomList.Instance.ChangeRoomToCreateName(text.text);
    }
    private string SetRoomName(int _num)
    {
        string[] roomNames =
        {
            "�ʺ��� ������",
            "�ųʰ� �ؿ�",
            "����",
            "Ʈ�������� ����",
            "�� T��?",
            "�������� ���Ժ��̿�",
            "�̰� ���� MZ�ε���?",
            "���� ����",
            "��� 50 �̸� ����",
            "�ƿ� �丮��ġ"
        };

        return roomNames[_num];
    }
}
