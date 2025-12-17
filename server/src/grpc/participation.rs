use api::pb::participation_service_server::ParticipationService;
use api::pb::{
    CreateParticipationRequest, DeleteParticipationRequest, GetParticipationRequest,
    ListParticipationsRequest, ListParticipationsResponse, Participation,
    UpdateParticipationRequest,
};
use tonic::{Request, Response, Result, Status};

#[derive(Debug, Default)]
pub struct ParticipationServer;

#[tonic::async_trait]
impl ParticipationService for ParticipationServer {
    async fn create_participation(
        &self,
        _request: Request<CreateParticipationRequest>,
    ) -> Result<Response<Participation>, Status> {
        todo!()
    }

    async fn get_participation(
        &self,
        _request: Request<GetParticipationRequest>,
    ) -> Result<Response<Participation>, Status> {
        todo!()
    }

    async fn list_participations(
        &self,
        _request: Request<ListParticipationsRequest>,
    ) -> Result<Response<ListParticipationsResponse>, Status> {
        todo!()
    }

    async fn update_participation(
        &self,
        _request: Request<UpdateParticipationRequest>,
    ) -> Result<Response<Participation>, Status> {
        todo!()
    }

    async fn delete_participation(
        &self,
        _request: Request<DeleteParticipationRequest>,
    ) -> Result<Response<()>, Status> {
        todo!()
    }
}
