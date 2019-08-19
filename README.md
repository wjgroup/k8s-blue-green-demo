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

## 2. Deploy(install) Ambassador on cluster into separate namespace `ambassador`
```
$ kubectl create namespace ambassador
$ kubectl -n ambassador apply -f k8s/ambassador-deploy-rbac.yaml
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

>Above the `ambassador-deploy-rbac.yaml` is a copy from [official site](https://getambassador.io/yaml/ambassador/ambassador-rbac.yaml), the only change I made is for `clusterrolebinding.rbac.authorization.k8s.io/ambassador` to bind service account from namespace `ambassador` instead of `default`, this is to demo one shared ambassador deployment serving multiple application namespaces.

## 3. Create Ambassador load balancer
```
$ kubectl -n ambassador apply -f k8s/ambassador-loadbalancer.yaml
```

After that, keep running below the command till you see EXTERNAL-IP is generated
```
$ kubectl -n ambassador get svc -o wide ambassador
NAME         TYPE           CLUSTER-IP   EXTERNAL-IP     PORT(S)        AGE   SELECTOR
ambassador   LoadBalancer   10.0.65.32   52.183.39.233   80:30958/TCP   36m   service=ambassador
```
>Note: both the Ambassador and load balancer need to be deployed into same namespace.

## 4. Deploy 2 demo apps in to `apps` namespace
```
$ kubectl create namespace apps
$ kubectl -n apps apply -f k8s/service-deploy-A.yaml
$ kubectl -n apps apply -f k8s/mapping-A.yaml
$ kubectl -n apps apply -f k8s/service-deploy-B.yaml
$ kubectl -n apps apply -f k8s/mapping-B.yaml
```

>There are 2 mapping yamls: `mapping-B.yaml` and `mapping-B-https.yaml`, the `mapping-B-https.yaml` forces ambassador to originate HTTPS connection with service/pods, which will be used when later we try out HTTPS related features. 

After deployment you should be able to use below the commands to access them
```
$ curl http://<external ip>/api/api/values
$ curl http://<external ip>/api-b/api/values
```

## 5. Switch url for service A(/api/api/values) to point to service B
```
$ kubectl apply -f k8s/mapping-A-to-B.yaml
$ curl http://<external ip>/api/api/values
```

## 6. Enabled HTTPS

Please read [the official article](https://www.getambassador.io/user-guide/tls-termination/) for full details of enable HTTPS for ambassador. To summarise, basically you need run below the command lines to create private key and cert, and then create tls secret on it and create tls context for ambassador to consume the secret
```
$ openssl genrsa -out key.pem 2048
$ openssl req -x509 -key key.pem -out cert.pem -days 365 -subj '/CN=ambassador-cert'
$ kubectl -n apps create secret tls tls-cert --cert=cert.pem --key=key.pem
$ kubectl -n apps apply -f k8s/tls-context.yaml
```
>Be aware, once enabled HTTPS the http port will not work.

>If you are using MacOS Mojave then above the `openssl req -x509` command may not work, you can then use the cert.pem and key.pem from the k8s folder directly. 

>Be aware the secret and tls context is not deployed into namespace `ambassador`, this is to demo one shared ambassador deployment serving multiple application namespaces.