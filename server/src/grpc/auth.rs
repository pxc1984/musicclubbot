use std::time::{SystemTime, UNIX_EPOCH};

use api::pb::{LoginResponse, TgLogin, auth_service_server::AuthService};
use jsonwebtoken::{Algorithm, DecodingKey, EncodingKey, Header, Validation, decode, encode};
use serde::{Deserialize, Serialize};
use tonic::{Request, Response, Result, Status};

#[derive(Debug, Serialize, Deserialize)]
struct Claims {
    sub: String,
    exp: usize,
    iat: usize,
}

#[derive(Debug)]
struct Keys {
    encoding: EncodingKey,
    decoding: DecodingKey,
}

impl Keys {
    fn new(secret: &[u8]) -> Self {
        Self {
            encoding: EncodingKey::from_secret(secret),
            decoding: DecodingKey::from_secret(secret),
        }
    }
}

#[derive(Debug)]
pub struct AuthServer {
    keys: Keys,
}

const TTL_SECONDS: usize = 60 * 60;

impl AuthServer {
    pub fn new(secret_key: &[u8]) -> Self {
        Self {
            keys: Keys::new(secret_key),
        }
    }

    fn now_ts() -> usize {
        SystemTime::now()
            .duration_since(UNIX_EPOCH)
            .expect("time went backwards")
            .as_secs() as usize
    }

    pub fn sign(&self, payload: String) -> String {
        let now = Self::now_ts();
        let claims = Claims {
            sub: payload,
            iat: now,
            exp: now + TTL_SECONDS,
        };
        encode(&Header::default(), &claims, &self.keys.encoding).expect("jwt encode failed")
    }

    pub fn check(&self, token: String) -> bool {
        let mut validation = Validation::new(Algorithm::HS256);
        validation.validate_exp = true;
        decode::<Claims>(&token, &self.keys.decoding, &validation).is_ok()
    }
}

#[tonic::async_trait]
impl AuthService for AuthServer {
    async fn login_tg(&self, request: Request<TgLogin>) -> Result<Response<LoginResponse>, Status> {
        let tg_id = request.into_inner().tg_id;
        if tg_id == 0 {
            return Err(Status::invalid_argument("tg_id must be non-zero"));
        }
        let token = self.sign(tg_id.to_string());
        Ok(Response::new(LoginResponse { token }))
    }
}
