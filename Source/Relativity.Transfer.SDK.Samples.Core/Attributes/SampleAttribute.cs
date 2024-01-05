using System;

namespace Relativity.Transfer.SDK.Samples.Core.Attributes;

[AttributeUsage(AttributeTargets.Class)]
internal sealed class SampleAttribute(
	int order,
	string menuCaption,
	string sampleDescription,
	Type sampleType,
	TransferType transferType)
	: Attribute, IEquatable<SampleAttribute>
{
	internal static readonly SampleAttribute ExitOptionAttribute =
		new(int.MaxValue, "Exit", "Options allows to exit application.", null, TransferType.Default);

	public int Order { get; } = order;
	public string MenuCaption { get; } = menuCaption;
	public string SampleDescription { get; } = sampleDescription;
	public Type SampleType { get; } = sampleType;
	public TransferType TransferType { get; } = transferType;

	internal bool IsExitOption => Equals(ExitOptionAttribute);

	public bool Equals(SampleAttribute other)
	{
		if (other is null) return false;
		if (ReferenceEquals(this, other)) return true;

		return base.Equals(other) && Order == other.Order &&
		       MenuCaption == other.MenuCaption &&
		       SampleDescription == other.SampleDescription &&
		       SampleType == other.SampleType &&
		       TransferType == other.TransferType;
	}

	public override string ToString()
	{
		return MenuCaption;
	}

	public override bool Equals(object obj)
	{
		return ReferenceEquals(this, obj) || (obj is SampleAttribute other && Equals(other));
	}

	public override int GetHashCode()
	{
		return (Order, MenuCaption, SampleDescription, SampleType, Direction: TransferType).GetHashCode();
	}
}