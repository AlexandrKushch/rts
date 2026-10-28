using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class SelectionController : Node2D
{
	private double _clickTimer = 0.0f;
	private PlayerBase PlayerBase;
	private SelectArea _selectArea;
	private MeshInstance2D _marker;
	[Export] private PackedScene SelectAreaScene;

	[Signal] public delegate void SelectionChangedEventHandler();

	public HashSet<SelectableComponent> Selections { get; private set; }

	public override void _Ready()
	{
		PlayerBase = GetParent<PlayerBase>();
		Selections = new HashSet<SelectableComponent>();
	}

	public override void _UnhandledInput(InputEvent input)
	{
		if (PlayerBase.BuildingController.BlueprintActive) return;

		if (input is InputEventMouseButton buttonInput)
		{
			if (buttonInput.ButtonIndex == MouseButton.Left)
			{
				SelectionInput(buttonInput);
			}
			else if (buttonInput.Pressed
				&& buttonInput.ButtonIndex == MouseButton.Middle)
			{
				if (TryPointCastSelectable(out var selection)
					&& selection.EffectedOn is IDestroyableWithHp unit)
				{
					unit.TakeDamage(1);
					GD.Print(unit.HP);
				}
			}
		}
	}

	private void SelectionInput(InputEventMouseButton input)
	{
		if (!input.Pressed)
		{
			if (TryPointCastSelectable(out var selection))
			{				
				if (selection.EffectedOn is UnitBase)
				{
					ProcessUnitSelection(selection);
				}
				else
				{
					if (selection.EffectedOn is BuildingBase building
						&& PlayerBase is HumanPlayer humanPlayer)
					{
						humanPlayer.CameraController.AlignWith(building.GlobalPosition);
					}
					
					ClearSelections();
					Selections.Add(selection);
				}
			}
			else
			{
				ClearSelections();
			}

			SelectUnits();
			EmitSignal(SignalName.SelectionChanged);
		}
	}

	public void ClearUnitsExcept(UnitTypeIds unitId)
	{
		foreach (var unit in Selections)
		{
			unit.UpdateSelection(false);
		}

		Selections = Selections
			.Where(x => x.EffectedOn is UnitBase unit && unit.Meta.Id == unitId)
			.Where(x => x != null)
			.ToHashSet();

		foreach (var unit in Selections)
		{
			unit.UpdateSelection(true);
		}

		EmitSignal(SignalName.SelectionChanged);
	}

	public void SelectFormation(Formation formation)
	{
		ClearSelections();
		Selections = new HashSet<SelectableComponent> { formation.UnitSelectableComponent };
	}

	public void ClearSelections()
	{
		foreach (var selection in Selections)
		{
			selection.UpdateSelection(false);
		}

		Selections.Clear();
	}

	private void ProcessUnitSelection(SelectableComponent selection)
	{
		var toDeselect = Selections.Where(x => x.EffectedOn is not UnitBase).ToHashSet();

		foreach (var x in toDeselect)
		{
			x.UpdateSelection(false);
			Selections.Remove(x);
		}

		if (Selections.Contains(selection))
		{
			if (Selections.Count == 1)
			{
				if (selection.EffectedOn is IHasFormationUnit hasFormationUnit
					&& hasFormationUnit.Formation != null)
				{
					SelectFormation(hasFormationUnit.Formation);
				}
				else
				{
					var selectables = QueryNearUnits(selection.GlobalPosition, 200);

					foreach (var selectable in selectables)
					{
						Selections.Add(selectable);
					}
				}
			}
			else
			{
				selection.UpdateSelection(false);
				Selections.Remove(selection);
			}
		}
		else if (selection.EffectedOn is IHasFormationUnit hasFormationUnit
			&& hasFormationUnit.Formation != null)
		{
			ClearSelections();
			SelectFormation(hasFormationUnit.Formation);
		}
		else
		{
			Selections.Add(selection);
		}
	}

	private void SelectUnits()
	{
		var hasUnits = Selections.Any(x => x.EffectedOn is UnitBase);

		if (hasUnits)
		{
			Selections = Selections.Where(x => x.EffectedOn is UnitBase && x != null).ToHashSet();
		}

		foreach (var selection in Selections)
		{
			selection.UpdateSelection(true);
		}
	}

	private SelectableComponent[] QueryNearUnits(Vector2 from, float distance)
	{
		var nodes = GlobalResources.Instance.World
			.GetChildren()
			.Where(x => x is UnitBase
				&& (x is not IHasTeam hasTeam || hasTeam.Team == PlayerBase.Team)
				&& from.DistanceTo((x as Node2D).GlobalPosition) < distance
				&& (x is not IHasFormationUnit hasFormation || hasFormation.Formation == null));

		var selectables = nodes
			.Select(x => x.GetNode<SelectableComponent>(nameof(SelectableComponent)))
			.ToArray();

		return selectables;
	}

	private SelectableComponent[] QuerySelectablesOfType(Type type, Vector2 from, float distance)
	{
		var nodes = GlobalResources.Instance.World
			.GetChildren()
			.Where(x => x.GetType() == type && from.DistanceTo((x as Node2D).GlobalPosition) < distance);

		var selectables = nodes
			.Select(x => x.GetNode<SelectableComponent>(nameof(SelectableComponent)))
			.ToArray();

		return selectables;
	}

	private bool TryPointCastSelectable(out SelectableComponent collider)
	{
		collider = null;

		var spaceState = GetWorld2D().DirectSpaceState;
		var mouse = GetGlobalMousePosition();
		var query = new PhysicsPointQueryParameters2D
		{
			Position = mouse,
			CollideWithAreas = true,
			CollideWithBodies = false,
			CollisionMask = 512
		};

		var results = spaceState.IntersectPoint(query);
		var selectables = results
			.Select(x => x["collider"].As<SelectableComponent>())
			.Where(
				x => x != null
					&& (x.EffectedOn is not IHasTeam hasTeam || hasTeam.Team == PlayerBase.Team))
			.OrderBy(item => item.EffectedOn switch
			{
				UnitBase => 0,
				ResourceBase => 1,
				BuildingBase => 2,
				_ => 3
			})
			.ThenBy(x => (x.EffectedOn as Node2D)?.GlobalPosition.DistanceTo(mouse) ?? float.MaxValue)
			.ThenBy(x => !x.Selected)
			.ToArray();

		if (selectables.Length > 0)
		{
			collider = selectables[0];
			return true;
		}

		return false;
	}
}
