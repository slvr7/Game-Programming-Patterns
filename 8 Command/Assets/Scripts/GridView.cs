using System;
using System.Collections.Generic;
using UnityEngine;

namespace GPP
{
    public class GridView : MonoBehaviour
    {
        [SerializeField] private GameObject _redGem;
        [SerializeField] private GameObject _greenGem;
        [SerializeField] private GameObject _blueGem;
        [SerializeField] private GameObject _yellowGem;
        [SerializeField] private float _shrinkDuration;
        [SerializeField] private float _growDuration;
        [SerializeField] private float _moveDuration;

        private const int WIDTH = 16;
        private const int HEIGHT = 9;

        private readonly SerialTasks _animations = new SerialTasks();

        private readonly Commands _commands = new Commands();

        private readonly Dictionary<Gem.Kind, GameObject> _gems = new Dictionary<Gem.Kind, GameObject>();
        private readonly GemGrid _grid = new GemGrid(WIDTH, HEIGHT);

        private void Awake()
        {
            _gems[Gem.Kind.Red] = _redGem;
            _gems[Gem.Kind.Yellow] = _yellowGem;
            _gems[Gem.Kind.Blue] = _blueGem;
            _gems[Gem.Kind.Green] = _greenGem;
        }

        private void Start()
        {
            var changes = _grid.Init();
            HandleGridChanges(changes);
        }

        private readonly Dictionary<ulong, GemView> _gemViews = new Dictionary<ulong, GemView>();

        private void Update()
        {
            _animations.Update();

            // Ignore input if there are any animations still happening
            if (!_animations.HasTasks)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    var mp = transform.worldToLocalMatrix * Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    var x = (int) mp.x;
                    var y = (int) mp.y;

                    _commands.Do(new ClickCommand(new Vector2Int(x, y), this));
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    _commands.Undo();
                }
                else if (_gemViews.Count == 0)
                {
                    // Reset the game
                    _commands.Clear();
                    HandleGridChanges(_grid.Init());
                }
            }
        }

        private void Click(Vector2Int pos)
        {
            var changes = _grid.ClearGroupAt(pos.x, pos.y);
            HandleGridChanges(changes);
        }

        private CreateGemAnimation CreateGemView(Gem gem, Vector2Int pos)
        {
            var gemView = Instantiate(_gems[gem.Type], gameObject.transform, true);
            var gemBehavior = gemView.GetComponent<GemView>();
            _gemViews.Add(gem.Id, gemBehavior);
            gemView.transform.localPosition = new Vector3(pos.x, pos.y, 0);
            return new CreateGemAnimation(gemBehavior, _growDuration);
        }

        private DestroyGemAnimation DestroyGemView(GemView gemView)
        {
            return new DestroyGemAnimation(gemView, _shrinkDuration);
        }

        private MoveGemAnimation MoveGemView(GemView gemView, Vector2Int pos)
        {
            return new MoveGemAnimation(gemView, pos, _moveDuration);
        }

        private void HandleGridChanges(IEnumerable<GemGrid.Change> changes)
        {
            // We want animations of different types to happen all at once so we're going
            // to organize them into distinct task groups that are sequenced in the order we need
            var destroyAnimations = new ParallelTasks();
            var moveAnimations = new ParallelTasks();
            var createAnimations = new ParallelTasks();

            foreach (var change in changes)
            {
                switch (change.Type)
                {
                    case GemGrid.Change.Kind.Created:
                        var createAnim = CreateGemView(change.Gem, change.Position);
                        createAnimations.Add(createAnim);
                        break;

                    case GemGrid.Change.Kind.Destroyed:
                        var destroyAnim = DestroyGemView(_gemViews[change.Gem.Id]);
                        destroyAnimations.Add(destroyAnim);
                        _gemViews.Remove(change.Gem.Id);
                        break;

                    case GemGrid.Change.Kind.Moved:
                        var moveAnim = MoveGemView(_gemViews[change.Gem.Id], change.Position);
                        moveAnimations.Add(moveAnim);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            _animations.Add(
                // first we destroy any gems to make room...
                destroyAnimations,
                // then we move them into place...
                moveAnimations,
                // and then create new oness
                createAnimations
            );
        }

        private abstract class TimedTask : Task
        {
            private float _elapsedTime;
            private readonly float _duration;

            protected TimedTask(float duration)
            {
                _duration = duration;
            }

            internal override void Update()
            {
                _elapsedTime += Time.deltaTime;
                var t = _elapsedTime / _duration;
                Tick(t);
                if (t >= 1) SetStatus(TaskStatus.Success);
            }

            protected abstract void Tick(float t);
        }

        private class MoveGemAnimation : TimedTask
        {
            private readonly GemView _gemView;
            private Vector3 _start;
            private readonly Vector3 _end;

            public MoveGemAnimation(GemView gemView, Vector2Int end, float duration) : base(duration)
            {
                _gemView = gemView;
                _end = new Vector3(end.x, end.y, 0);
            }

            protected override void Init()
            {
                _start = _gemView.transform.localPosition;
            }

            protected override void Tick(float t)
            {
                _gemView.transform.localPosition = Vector3.Lerp(_start, _end, Easing.BounceEaseOut(t));
            }
        }

        private class CreateGemAnimation : TimedTask
        {
            private readonly GemView _gemView;

            public CreateGemAnimation(GemView gemView, float duration) : base(duration)
            {
                _gemView = gemView;
                _gemView.transform.localScale = Vector3.zero;
            }

            protected override void Tick(float t)
            {
                _gemView.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, Easing.BounceEaseOut(t));
            }
        }

        private class DestroyGemAnimation : TimedTask
        {
            private readonly GemView _gemView;

            public DestroyGemAnimation(GemView gemView, float duration) : base(duration)
            {
                _gemView = gemView;
            }

            protected override void Tick(float t)
            {
                _gemView.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, Easing.QuadEaseIn(t));
            }

            protected override void CleanUp()
            {
                Destroy(_gemView);
            }
        }

        private class ClickCommand : Command
        {
            private readonly Gem[,] _previousState;
            private readonly Vector2Int _clickPos;
            private readonly GridView _view;

            public ClickCommand(Vector2Int clickPos, GridView view)
            {
                _view = view;
                _clickPos = clickPos;
                // Copy the current gem layout so we can restore it later
                _previousState = _view._grid.CopyGems();
            }

            public override void Do()
            {
                _view.Click(_clickPos);
            }

            public override void Undo()
            {
                _view.HandleGridChanges(_view._grid.Set(_previousState));
            }
        }

    }
}

