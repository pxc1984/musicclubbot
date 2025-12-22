package app

import (
	"context"
	"fmt"
	"net"
	"net/http"

	"github.com/apsdehal/go-logger"
	"github.com/improbable-eng/grpc-web/go/grpcweb"
	"golang.org/x/net/http2"
	"golang.org/x/net/http2/h2c"
	"google.golang.org/grpc"
	"google.golang.org/grpc/reflection"

	"musicclubbot/backend/internal/api"
	"musicclubbot/backend/internal/config"
)

// Run initializes and starts the gRPC server with stub handlers.
func Run(ctx context.Context) error {
	cfg := ctx.Value("cfg").(config.Config)
	log := ctx.Value("log").(*logger.Logger)
	lis, err := net.Listen("tcp", cfg.GRPCAddr())
	if err != nil {
		return fmt.Errorf("listen on %s: %w", cfg.GRPCAddr(), err)
	}

	grpcServer := grpc.NewServer(
		grpc.ChainUnaryInterceptor(
			withBaseContext(ctx),
			loggingInterceptor,
		),
	)

	api.Register(grpcServer)
	reflection.Register(grpcServer)

	grpcWeb := grpcweb.WrapServer(grpcServer, grpcweb.WithOriginFunc(func(origin string) bool {
		// Allow all origins for now; tighten when hosts are known.
		return true
	}))

	handler := h2c.NewHandler(http.HandlerFunc(func(w http.ResponseWriter, r *http.Request) {
		// Handle CORS preflight explicitly.
		if r.Method == http.MethodOptions {
			w.Header().Set("Access-Control-Allow-Origin", "*")
			w.Header().Set("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS")
			w.Header().Set("Access-Control-Allow-Headers", "Content-Type, X-Grpc-Web, X-User-Agent, Authorization")
			w.WriteHeader(http.StatusNoContent)
			return
		}

		if grpcWeb.IsGrpcWebRequest(r) || grpcWeb.IsGrpcWebSocketRequest(r) || grpcWeb.IsAcceptableGrpcCorsRequest(r) {
			grpcWeb.ServeHTTP(w, r)
			return
		}

		w.WriteHeader(http.StatusNotFound)
	}), &http2.Server{})

	srv := &http.Server{
		Handler: handler,
	}

	// Graceful stop on context cancellation.
	go func() {
		<-ctx.Done()
		grpcServer.GracefulStop()
		_ = srv.Shutdown(context.Background())
	}()

	log.Infof("Starting gRPC server on %s", cfg.GRPCAddr())
	if err := srv.Serve(lis); err != nil && err != http.ErrServerClosed {
		return fmt.Errorf("serve gRPC/gRPC-Web: %w", err)
	}

	return nil
}

// withBaseContext propagates shared values (cfg, log, db, etc.) from the parent context
// into every incoming request context so handlers can retrieve them.
func withBaseContext(base context.Context) grpc.UnaryServerInterceptor {
	return func(ctx context.Context, req interface{}, _ *grpc.UnaryServerInfo, handler grpc.UnaryHandler) (interface{}, error) {
		for _, key := range []string{"cfg", "log", "db"} {
			if v := base.Value(key); v != nil {
				ctx = context.WithValue(ctx, key, v)
			}
		}
		return handler(ctx, req)
	}
}
