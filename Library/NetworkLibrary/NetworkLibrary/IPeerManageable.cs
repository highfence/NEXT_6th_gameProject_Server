namespace NetworkLibrary
{
	// 세션 관리자 인터페이스.
	public interface ISessionManageable
	{
		// 해당 세션이 유효한지 확인해주는 메서드.
		bool IsUserExist(ISession owner);
	}
}
