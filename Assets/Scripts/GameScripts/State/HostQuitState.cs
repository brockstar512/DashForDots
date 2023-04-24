using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Unity.Netcode;

public class HostQuitState : BaseState
{
    [SerializeField] Scenes targetScene;
    [SerializeField] Button mainMenu;

    public override void Initialize(StateManager StateManager)
    {
        cg.DOFade(0, .1f).OnComplete(() => { this.GetPage.DOScale(Vector3.zero, 0); });

        mainMenu.onClick.AddListener(delegate { MainMenu(); });
    }
    public override void EnterState(StateManager stateManager)
    {
        this.GetPage.DOScale(Vector3.one, 0).OnComplete(() => { cg.DOFade(1, .25f); });
    }

    public override void UpdateState(StateManager stateManager)
    {

    }
    public override void LeaveState()
    {
        cg.DOFade(0, .1f).OnComplete(() => { this.GetPage.DOScale(Vector3.zero, 0); });
    }

    private void MainMenu()
    {
        if (MultiplayerController.Instance.IsMultiplayer)
        {
            MultiplayerController.Instance.ShutDown();
            Destroy(NetworkManager.Singleton.gameObject);
            Destroy(MultiplayerController.Instance.gameObject);
        }
        LoadingManager.Instance.LoadScene(targetScene.ToString());

    }

}
