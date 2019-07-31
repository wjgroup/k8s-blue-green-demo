# Blue/green deployment demo with Ambassador

## Prerequisites

* create new Azure aks service
* have `kubectl` installed
* have Azure CLI installed

## 1. Connect to Azure aks cluster through `kubectl`
```
az login
az aks get-credentials --resource-group <group_name> --name <cluster_name>
kubectl version
```

## 2. Deploy(install) Ambassador on cluster
```
$ kubectl apply -f https://getambassador.io/yaml/ambassador/ambassador-rbac.yaml
service/ambassador-admin created
clusterrole.rbac.authorization.k8s.io/ambassador created
serviceaccount/ambassador created
clusterrolebinding.rbac.authorization.k8s.io/ambassador created
customresourcedefinition.apiextensions.k8s.io/authservices.getambassador.io created
customresourcedefinition.apiextensions.k8s.io/mappings.getambassador.io created
customresourcedefinition.apiextensions.k8s.io/modules.getambassador.io created
customresourcedefinition.apiextensions.k8s.io/ratelimitservices.getambassador.io created
customresourcedefinition.apiextensions.k8s.io/tcpmappings.getambassador.io created
customresourcedefinition.apiextensions.k8s.io/tlscontexts.getambassador.io created
customresourcedefinition.apiextensions.k8s.io/tracingservices.getambassador.io created
```

## 3. Create Ambassador load balancer
```
$ kubectl apply -f https://raw.githubusercontent.com/wjgroup/k8s-ingress-demo/master/ambassador-demo/ambassador-loadbalancer.yaml
```

After than, keep running below the command till you see EXTERNAL-IP is generated
```
$ kubectl get svc -o wide ambassador
NAME         TYPE           CLUSTER-IP   EXTERNAL-IP     PORT(S)        AGE   SELECTOR
ambassador   LoadBalancer   10.0.65.32   52.183.39.233   80:30958/TCP   36m   service=ambassador
```
>Note: both the Ambassador and load balancer are deployed in default namespace. Deploying them to different namespaces requires extra config changes.

## 4. Deploy 2 demo apps in to `apps` namespace
```
$ kubectl create namespace apps
$ kubectl -n apps apply -f k8s/service-deploy-A.yaml
$ kubectl -n apps apply -f k8s/mapping-A.yaml
$ kubectl -n apps apply -f k8s/service-deploy-B.yaml
$ kubectl -n apps apply -f k8s/mapping-B.yaml
```

After deployed you should be able to use below the commands to access them
```
$ curl http://<external ip>/api/api/values
$ curl http://<external ip>/api-b/api/values
```

## 5. Switch url for service A(/api/api/values) to point to service B
```
$ kubectl apply -f k8s/mapping-A-to-B.yaml
$ curl http://<external ip>/api/api/values
```

