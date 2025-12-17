use api::pb::{
    LoginResponse, TgLogin,
    auth_server::{self, Auth},
};
use env_logger::Env;
use tonic::{Request, Response, Result, Status, transport::Server};

#[derive(Debug, Default)]
struct AuthServer;

#[tonic::async_trait]
impl Auth for AuthServer {
    async fn login_tg(
        &self,
        _request: Request<TgLogin>,
    ) -> Result<Response<LoginResponse>, Status> {
        todo!()
    }
}

#[tokio::main]
async fn main() -> Result<(), Box<dyn std::error::Error>> {
    env_logger::Builder::from_env(Env::default().default_filter_or("debug")).init();
    let addr = "[::1]:6969".parse()?;
    let server = AuthServer::default();

    log::info!("Server is running at {addr}");
    Server::builder()
        .add_service(auth_server::AuthServer::new(server))
        .serve(addr)
        .await
        .unwrap();

    Ok(())
}
