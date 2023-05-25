using DG.Tweening;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

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
            Destroy(GameLobby.Instance.gameObject);
        }
        LoadingManager.Instance.LoadScene(targetScene.ToString());

    }

}
