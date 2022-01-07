$version = 8
docker build -f .\WebApplication5\Dockerfile -t crgarakspublicacr.azurecr.io/webapp5:$version .
docker push crgarakspublicacr.azurecr.io/webapp5:$version

az acr login -n crgarakspublicacr
kubectl apply -f .\deployment-linux.yaml

kubectl get events --sort-by='.metadata.creationTimestamp' -A

kubectl run -i --tty busybox --image=busybox --restart=Never -- sh