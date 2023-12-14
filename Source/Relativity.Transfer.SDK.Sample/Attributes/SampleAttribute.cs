﻿using System;

namespace Relativity.Transfer.SDK.Sample.Attributes;

[AttributeUsage(AttributeTargets.Class)]
internal sealed class SampleAttribute : Attribute, IEquatable<SampleAttribute>
{
	internal static readonly SampleAttribute ExitOptionAttribute =
		new(int.MaxValue, "Exit", "Options allows to exit application.", null, TransferType.Default);

	public SampleAttribute(int order,
		string menuCaption,
		string sampleDescription,
		Type sampleType,
		TransferType transferType)
	{
		Order = order;
		MenuCaption = menuCaption;
		SampleDescription = sampleDescription;
		SampleType = sampleType;
		TransferType = transferType;
	}

	public int Order { get; }
	public string MenuCaption { get; }
	public string SampleDescription { get; }
	public Type SampleType { get; }
	public TransferType TransferType { get; }

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

	public override int GetHashCode() => (Order, MenuCaption, SampleDescription, SampleType, Direction: TransferType).GetHashCode();
}