export function LoginCard({ tgIdInput, setTgIdInput, loginStatus, onLogin, loading }: LoginCardProps) {
	return (
		<div className="card">
			<div className="section-header">
				<div className="card-title">
					<span className="status-dot" />
					Login with TG id
				</div>
				<span className="subtle">Required for secured calls</span>
			</div>
			<form
				className="grid"
				style={{ gap: 10 }}
				onSubmit={(e) => {
					e.preventDefault();
					if (!tgIdInput.trim()) return;
					onLogin(tgIdInput.trim());
				}}
			>
				<input
					className="input"
					placeholder="Telegram user id"
					value={tgIdInput}
					onChange={(e) => setTgIdInput(e.target.value.replace(/[^0-9]/g, ""))} />
				<div style={{ display: "flex", gap: 10, alignItems: "center" }}>
					<button className="button" type="submit" disabled={loading || !tgIdInput}>
						{loading ? "Authorizing..." : "Login & set token"}
					</button>
					{loginStatus ? <span className="subtle">{loginStatus}</span> : null}
				</div>
			</form>
		</div>
	);
} export type LoginCardProps = {
	tgIdInput: string;
	setTgIdInput: (val: string) => void;
	loginStatus: string | null;
	onLogin: (tg: string) => void;
	loading: boolean;
};

