using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelState
{
    public int TextureID;
    public bool Active;
    public PixelState(int TextureID, bool Active) {
        this.TextureID = TextureID;
        this.Active = Active;
    }
    public static bool operator ==(PixelState a, PixelState b) {
        if (object.ReferenceEquals(null, a))
            return object.ReferenceEquals(null, b);
        if (object.ReferenceEquals(null, b))
            return false;
        return (a.TextureID == b.TextureID && a.Active == b.Active);
    }
    public static bool operator !=(PixelState a, PixelState b)
        => !(a == b);
	public override bool Equals(object obj)
	{
		var item = obj as PixelState;

		if (item == null) {
			return false;
		}

        return this == item;
	}
	public override int GetHashCode()
	{
		return TextureID*2+(Active?1:0);
	}
}
